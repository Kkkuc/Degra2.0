using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class TimetableCrawler
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;
        private readonly HtmlScraper _htmlScraper;

        public TimetableCrawler(AppDbContext context)
        {
            _context = context;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
            _htmlScraper = new HtmlScraper();
        }

        public async Task CrawlAndSaveAsync()
        {
            string baseUrl = "https://degra.wi.pb.edu.pl/rozklady/rozklad.php?page=nau";
            string mainPageHtml = await _httpClient.GetStringAsync(baseUrl);
            var mainDoc = new HtmlDocument();
            mainDoc.LoadHtml(mainPageHtml);

            var optionNodes = mainDoc.DocumentNode.SelectNodes("//select[@id='teacher']/option");
            if (optionNodes == null) return;

            var teacherIds = optionNodes.Select(n => n.GetAttributeValue("value", "")).Where(id => !string.IsNullOrEmpty(id)).ToList();

            var defaultDepartment = await _context.Faculties.FirstOrDefaultAsync(d => d.Name == "Wydział Informatyki PB");
            if (defaultDepartment == null)
            {
                defaultDepartment = new Faculty { Name = "Wydział Informatyki PB", Abbreviation = "WI" };
                _context.Faculties.Add(defaultDepartment);
                await _context.SaveChangesAsync();
            }

            var defaultAcademicYear = await _context.AcademicYears.FirstOrDefaultAsync(a => a.Name == "Bieżący");
            if (defaultAcademicYear == null)
            {
                defaultAcademicYear = new AcademicYear { Name = "Bieżący", StartDate = DateTime.Now, EndDate = DateTime.Now.AddYears(1) };
                _context.AcademicYears.Add(defaultAcademicYear);
                await _context.SaveChangesAsync();
            }

            var defaultBuilding = await _context.Buildings.FirstOrDefaultAsync(b => b.Name == "Główny");
            if (defaultBuilding == null)
            {
                defaultBuilding = new Building { Name = "Główny", Address = "Białystok" };
                _context.Buildings.Add(defaultBuilding);
                await _context.SaveChangesAsync();
            }

            foreach (var id in teacherIds)
            {
                string teacherUrl = $"https://degra.wi.pb.edu.pl/rozklady/rozklad.php?id={id}&page=nau";
                
                try
                {
                    string teacherHtml = await _httpClient.GetStringAsync(teacherUrl);
                    var scrapedData = _htmlScraper.ParseTimetable(teacherHtml);
                    await SaveToDatabaseAsync(scrapedData, defaultDepartment, defaultAcademicYear, defaultBuilding);
                }
                catch (Exception)
                {
                }
                
                await Task.Delay(500);
            }
        }

        private async Task SaveToDatabaseAsync(List<ScrapedTimetableDto> scrapedData, Faculty dept, AcademicYear year, Building building)
        {
            foreach (var entry in scrapedData)
            {
                var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.FirstName == entry.TeacherFirstName && t.LastName == entry.TeacherLastName);
                if (teacher == null)
                {
                    teacher = new Teacher { AcademicTitle = entry.TeacherTitle, FirstName = entry.TeacherFirstName, LastName = entry.TeacherLastName };
                    _context.Teachers.Add(teacher);
                    await _context.SaveChangesAsync();
                }

                var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Name == entry.SubjectName);
                if (subject == null)
                {
                    subject = new Subject { Name = entry.SubjectName, Abbreviation = new string(entry.SubjectName.Where(char.IsUpper).ToArray()), Code = "NONE" };
                    _context.Subjects.Add(subject);
                    await _context.SaveChangesAsync();
                }

                var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Number == entry.RoomNumber && r.BuildingId == building.Id);
                if (room == null)
                {
                    room = new Room { Number = entry.RoomNumber, BuildingId = building.Id, Type = "Sala", Capacity = 30 };
                    _context.Rooms.Add(room);
                    await _context.SaveChangesAsync();
                }

                var classType = await _context.ClassTypes.FirstOrDefaultAsync(c => c.Abbreviation == entry.ClassTypeAbbr);
                if (classType == null)
                {
                    classType = new ClassType { Abbreviation = entry.ClassTypeAbbr, Name = entry.ClassTypeAbbr };
                    _context.ClassTypes.Add(classType);
                    await _context.SaveChangesAsync();
                }

                var field = await _context.FieldsOfStudy.FirstOrDefaultAsync(f => f.Name == entry.FieldOfStudyName && f.Mode == entry.ModeDegree && f.DepartmentId == dept.Id);
                if (field == null)
                {
                    field = new FieldOfStudy { Name = entry.FieldOfStudyName, Mode = entry.ModeDegree, Degree = entry.ModeDegree, DepartmentId = dept.Id };
                    _context.FieldsOfStudy.Add(field);
                    await _context.SaveChangesAsync();
                }

                string semName = $"Semestr {entry.SemesterNumber}";
                var semester = await _context.Semesters.FirstOrDefaultAsync(s => s.Name == semName && s.AcademicYearId == year.Id);
                if (semester == null)
                {
                    semester = new Semester { Name = semName, AcademicYearId = year.Id, StartDate = year.StartDate, EndDate = year.EndDate };
                    _context.Semesters.Add(semester);
                    await _context.SaveChangesAsync();
                }

                var spec = await _context.Specializations.FirstOrDefaultAsync(s => s.Name == entry.SpecializationName && s.FieldOfStudyId == field.Id);
                if (spec == null)
                {
                    spec = new Specialization { Name = entry.SpecializationName, FieldOfStudyId = field.Id };
                    _context.Specializations.Add(spec);
                    await _context.SaveChangesAsync();
                }

                var group = await _context.Groups.FirstOrDefaultAsync(g => g.Name == entry.GroupName && g.SemesterId == semester.Id && g.SpecializationId == spec.Id && g.ClassTypeId == classType.Id);
                if (group == null)
                {
                    group = new Group { Name = entry.GroupName, SemesterId = semester.Id, SpecializationId = spec.Id, ClassTypeId = classType.Id };
                    _context.Groups.Add(group);
                    await _context.SaveChangesAsync();
                }

                bool timetableExists = await _context.Timetables.AnyAsync(t => 
                    t.TeacherId == teacher.Id && 
                    t.DayOfWeek == entry.DayOfWeek && 
                    t.StartTime == entry.StartTime && 
                    t.EndTime == entry.EndTime &&
                    t.WeekCycle == entry.WeekCycle);

                if (!timetableExists)
                {
                    var newTimetable = new Timetable
                    {
                        TeacherId = teacher.Id,
                        SubjectId = subject.Id,
                        RoomId = room.Id,
                        ClassTypeId = classType.Id,
                        GroupId = group.Id,
                        DayOfWeek = entry.DayOfWeek,
                        StartTime = entry.StartTime,
                        EndTime = entry.EndTime,
                        WeekCycle = entry.WeekCycle
                    };

                    _context.Timetables.Add(newTimetable);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}