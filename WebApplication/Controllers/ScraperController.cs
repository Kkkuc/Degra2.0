using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Services;
using WebApplication.Models;

namespace WebApplication.Controllers;

public class ScraperController(AppDbContext context) : Controller
{
    private readonly HtmlScraper _scraper = new();
    private readonly HttpClient _httpClient = new();

    public async Task<IActionResult> Index()
    {
        try
        {
            ViewBag.TeacherCount = await context.Teachers.CountAsync();
            ViewBag.SubjectCount = await context.Subjects.CountAsync();
            ViewBag.TimetableCount = await context.Timetables.CountAsync();
        }
        catch
        {
            ViewBag.TeacherCount = 0;
            ViewBag.SubjectCount = 0;
            ViewBag.TimetableCount = 0;
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RunScraper()
    {
        try
        {
            string listUrl = "https://degra.wi.pb.edu.pl/rozklady/rozklad.php?page=nau";
            var mainHtml = await _httpClient.GetStringAsync(listUrl);
            var mainDoc = new HtmlDocument();
            mainDoc.LoadHtml(mainHtml);

            var teacherOptions = mainDoc.DocumentNode.SelectNodes("//select[@id='teacher']/option");
            if (teacherOptions == null) return RedirectToAction("Index");

            var faculty = await context.Faculties.FirstOrDefaultAsync(f => f.Abbreviation == "WI") 
                          ?? new Faculty { Name = "Wydział Informatyki", Abbreviation = "WI" };
            if (faculty.Id == 0) context.Faculties.Add(faculty);

            var year = await context.AcademicYears.FirstOrDefaultAsync(y => y.Name == "2025/2026")
                       ?? new AcademicYear { Name = "2025/2026", StartDate = new DateOnly(2025, 10, 1), EndDate = new DateOnly(2026, 9, 30) };
            if (year.Id == 0) context.AcademicYears.Add(year);

            await context.SaveChangesAsync();

            var semester = await context.Semesters.FirstOrDefaultAsync(s => s.Name == "Letni" && s.AcademicYearId == year.Id)
                           ?? new Semester { Name = "Letni", AcademicYearId = year.Id, StartDate = new DateOnly(2026, 2, 20), EndDate = new DateOnly(2026, 6, 30) };
            if (semester.Id == 0) context.Semesters.Add(semester);

            await context.SaveChangesAsync();

            int totalSynced = 0;
            foreach (var option in teacherOptions)
            {
                string teacherIdStr = option.GetAttributeValue("value", "");
                if (string.IsNullOrEmpty(teacherIdStr) || teacherIdStr == "0" || teacherIdStr == "468") continue;

                string teacherUrl = $"https://degra.wi.pb.edu.pl/rozklady/rozklad.php?id={teacherIdStr}&page=nau";
                var teacherHtml = await _httpClient.GetStringAsync(teacherUrl);
                var entries = _scraper.ParseTimetable(teacherHtml);

                foreach (var dto in entries)
                {
                    var teacher = await context.Teachers.FirstOrDefaultAsync(t => t.LastName == dto.TeacherLastName && t.FirstName == dto.TeacherFirstName)
                        ?? new Teacher 
                        { 
                            FirstName = string.IsNullOrWhiteSpace(dto.TeacherFirstName) ? "N/A" : dto.TeacherFirstName, 
                            LastName = string.IsNullOrWhiteSpace(dto.TeacherLastName) ? "N/A" : dto.TeacherLastName,
                            AcademicTitle = string.IsNullOrWhiteSpace(dto.TeacherTitle) ? "mgr" : dto.TeacherTitle,
                            Email = $"{Guid.NewGuid().ToString().Substring(0,8)}@pb.edu.pl"
                        };
                    if (teacher.Id == 0) context.Teachers.Add(teacher);

                    var building = await context.Buildings.FirstOrDefaultAsync(b => b.Name == dto.BuildingName)
                        ?? new Building { Name = dto.BuildingName, FacultyId = faculty.Id, HouseNumber = "45A", Street = "Wiejska", City = "Białystok", PostalCode = "15-351"};
                    if (building.Id == 0) context.Buildings.Add(building);
                    await context.SaveChangesAsync();

                    var room = await context.Rooms.FirstOrDefaultAsync(r => r.RoomNumber == dto.RoomNumber)
                        ?? new Room { RoomNumber = dto.RoomNumber, BuildingId = building.Id, RoomType = Models.enums.RoomType.Other, Capacity = 30 };
                    if (room.Id == 0) context.Rooms.Add(room);

                    var subject = await context.Subjects.FirstOrDefaultAsync(s => s.Name == dto.SubjectName)
                        ?? new Subject 
                        { 
                            Name = dto.SubjectName, 
                            Abbreviation = dto.SubjectName.Length > 20 ? dto.SubjectName.Substring(0, 20) : dto.SubjectName,
                            Code = "GEN-000"
                        };
                    if (subject.Id == 0) context.Subjects.Add(subject);

                    var fost = await context.FieldsOfStudy.FirstOrDefaultAsync(f => f.Name == dto.FieldOfStudyName)
                               ?? new FieldOfStudy { Name = dto.FieldOfStudyName, FacultyId = faculty.Id, Degree = "I stopień", Mode = "Stacjonarne" };
                    if (fost.Id == 0) context.FieldsOfStudy.Add(fost);

                    await context.SaveChangesAsync();

                    var group = await context.Groups.FirstOrDefaultAsync(g => g.Name == dto.GroupName && g.SemesterId == semester.Id)
                                ?? new Group { Name = dto.GroupName, SemesterId = semester.Id, FieldOfStudyId = fost.Id, ClassType = dto.ClassType };
                    if (group.Id == 0) context.Groups.Add(group);

                    await context.SaveChangesAsync();

                    var exists = await context.Timetables.AnyAsync(t => 
                        t.TeacherId == teacher.Id && 
                        t.DayOfWeek == dto.DayOfWeek && 
                        t.StartTime == dto.StartTime &&
                        t.SubjectId == subject.Id);
                    if (!exists)
                    {
                        context.Timetables.Add(new Timetable
                        {
                            SubjectId = subject.Id,
                            TeacherId = teacher.Id,
                            RoomId = room.Id,
                            GroupId = group.Id,
                            DayOfWeek = dto.DayOfWeek,
                            StartTime = dto.StartTime,
                            EndTime = dto.EndTime,
                            ClassType = dto.ClassType,
                            WeekCycle = dto.WeekCycle
                        });
                        totalSynced++;
                    }
                }
                await Task.Delay(150);
            }

            await context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Successfully synchronized {totalSynced} new entries.";
        }
        catch
        {
            TempData["ErrorMessage"] = "Database connection failed. Synchronization aborted.";
        }
        return RedirectToAction("Index");
    }
}