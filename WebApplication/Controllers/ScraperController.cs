using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Services;
using WebApplication.Models;

namespace WebApplication.Controllers;

public class ScraperController : Controller
{
    private readonly AppDbContext _context;
    private readonly HtmlScraper _scraper;
    private readonly HttpClient _httpClient;

    public ScraperController(AppDbContext context)
    {
        _context = context;
        _scraper = new HtmlScraper();
        _httpClient = new HttpClient();
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.TeacherCount = await _context.Teachers.CountAsync();
        ViewBag.SubjectCount = await _context.Subjects.CountAsync();
        ViewBag.TimetableCount = await _context.Timetables.CountAsync();
        return View();
    }

[HttpPost]
public async Task<IActionResult> RunScraper()
{
    string listUrl = "https://degra.wi.pb.edu.pl/rozklady/rozklad.php?page=nau";
    var mainHtml = await _httpClient.GetStringAsync(listUrl);
    var mainDoc = new HtmlDocument();
    mainDoc.LoadHtml(mainHtml);

    var teacherOptions = mainDoc.DocumentNode.SelectNodes("//select[@id='teacher']/option");
    if (teacherOptions == null) return RedirectToAction("Index");

    var faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.Abbreviation == "WI") 
                  ?? new Faculty { Name = "Wydział Informatyki", Abbreviation = "WI" };
    if (faculty.Id == 0) _context.Faculties.Add(faculty);

    var year = await _context.AcademicYears.FirstOrDefaultAsync(y => y.Name == "2025/2026")
               ?? new AcademicYear { Name = "2025/2026", StartDate = new DateTime(2025, 10, 1), EndDate = new DateTime(2026, 9, 30) };
    if (year.Id == 0) _context.AcademicYears.Add(year);

    await _context.SaveChangesAsync();

    var semester = await _context.Semesters.FirstOrDefaultAsync(s => s.Name == "Letni" && s.AcademicYearId == year.Id)
                   ?? new Semester { Name = "Letni", AcademicYearId = year.Id, StartDate = new DateTime(2026, 2, 20), EndDate = new DateTime(2026, 6, 30) };
    if (semester.Id == 0) _context.Semesters.Add(semester);

    await _context.SaveChangesAsync();

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
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.LastName == dto.TeacherLastName && t.FirstName == dto.TeacherFirstName)
                ?? new Teacher 
                { 
                    FirstName = string.IsNullOrWhiteSpace(dto.TeacherFirstName) ? "N/A" : dto.TeacherFirstName, 
                    LastName = string.IsNullOrWhiteSpace(dto.TeacherLastName) ? "N/A" : dto.TeacherLastName,
                    AcademicTitle = string.IsNullOrWhiteSpace(dto.TeacherTitle) ? "mgr" : dto.TeacherTitle,
                    Email = $"{Guid.NewGuid().ToString().Substring(0,8)}@pb.edu.pl"
                };
            if (teacher.Id == 0) _context.Teachers.Add(teacher);

            var building = await _context.Buildings.FirstOrDefaultAsync(b => b.Name == dto.BuildingName)
                ?? new Building { Name = dto.BuildingName, FacultyId = faculty.Id, Address = "Wiejska 45A" };
            if (building.Id == 0) _context.Buildings.Add(building);

            await _context.SaveChangesAsync();

            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomNumber == dto.RoomNumber)
                ?? new Room { RoomNumber = dto.RoomNumber, BuildingId = building.Id, RoomType = Models.enums.RoomType.Other, Capacity = 30 };
            if (room.Id == 0) _context.Rooms.Add(room);

            var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Name == dto.SubjectName)
                ?? new Subject 
                { 
                    Name = dto.SubjectName, 
                    Abbreviation = dto.SubjectName.Length > 20 ? dto.SubjectName.Substring(0, 20) : dto.SubjectName,
                    Code = "GEN-000"
                };
            if (subject.Id == 0) _context.Subjects.Add(subject);

            var fost = await _context.FieldsOfStudy.FirstOrDefaultAsync(f => f.Name == dto.FieldOfStudyName)
                       ?? new FieldOfStudy { Name = dto.FieldOfStudyName, FacultyId = faculty.Id, Degree = "I stopień", Mode = "Stacjonarne" };
            if (fost.Id == 0) _context.FieldsOfStudy.Add(fost);

            await _context.SaveChangesAsync();

            var group = await _context.Groups.FirstOrDefaultAsync(g => g.Name == dto.GroupName && g.SemesterId == semester.Id)
                        ?? new Group { Name = dto.GroupName, SemesterId = semester.Id, FieldOfStudyId = fost.Id, ClassType = dto.ClassType };
            if (group.Id == 0) _context.Groups.Add(group);

            await _context.SaveChangesAsync();

            var exists = await _context.Timetables.AnyAsync(t => 
                t.TeacherId == teacher.Id && 
                t.DayOfWeek == dto.DayOfWeek && 
                t.StartTime == dto.StartTime &&
                t.SubjectId == subject.Id);

            if (!exists)
            {
                _context.Timetables.Add(new Timetable
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

    await _context.SaveChangesAsync();
    TempData["SuccessMessage"] = $"Successfully synchronized {totalSynced} new entries.";
    return RedirectToAction("Index");
}
}