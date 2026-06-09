using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Models.enums;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services.ModelServices;

public class ScraperService : IScraperService
{
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _context;
    private static readonly Dictionary<int, (TimeSpan Start, TimeSpan End)> TimeBlocks = new()
    {
        { 1, (new TimeSpan(8, 30, 0), new TimeSpan(9, 15, 0)) },
        { 2, (new TimeSpan(9, 15, 0), new TimeSpan(10, 0, 0)) },
        { 3, (new TimeSpan(10, 15, 0), new TimeSpan(11, 0, 0)) },
        { 4, (new TimeSpan(11, 0, 0), new TimeSpan(11, 45, 0)) },
        { 5, (new TimeSpan(12, 0, 0), new TimeSpan(12, 45, 0)) },
        { 6, (new TimeSpan(12, 45, 0), new TimeSpan(13, 30, 0)) },
        { 7, (new TimeSpan(14, 0, 0), new TimeSpan(14, 45, 0)) },
        { 8, (new TimeSpan(14, 45, 0), new TimeSpan(15, 30, 0)) },
        { 9, (new TimeSpan(16, 0, 0), new TimeSpan(16, 45, 0)) },
        { 10, (new TimeSpan(16, 45, 0), new TimeSpan(17, 30, 0)) },
        { 11, (new TimeSpan(17, 40, 0), new TimeSpan(18, 25, 0)) },
        { 12, (new TimeSpan(18, 25, 0), new TimeSpan(19, 10, 0)) },
        { 13, (new TimeSpan(19, 20, 0), new TimeSpan(20, 5, 0)) },
        { 14, (new TimeSpan(20, 05, 0), new TimeSpan(20, 50, 0)) }
    };

    public ScraperService(HttpClient httpClient, AppDbContext context)
    {
        _httpClient = httpClient;
        _context = context;
    }

    public async Task ScrapeAndSaveAsync(string url)
    {
        var xmlContent = await _httpClient.GetStringAsync(url);
        var doc = XDocument.Parse(xmlContent);
        await ProcessXmlAsync(doc);
    }

    public async Task ImportFromFileAsync(Stream fileStream)
    {
        var doc = await XDocument.LoadAsync(fileStream, LoadOptions.None, default);
        await ProcessXmlAsync(doc);
    }

    private async Task ProcessXmlAsync(XDocument doc)
    {
        await EnsureDataConstraintsAsync();

        var academicTitles = ParseAcademicTitles(doc);
        await ProcessFieldsOfStudyAsync(doc);
        await ProcessSpecializationsAsync(doc);
        await ProcessRoomsAsync(doc);
        await ProcessTeachersAsync(doc, academicTitles);
        await ProcessSubjectsAsync(doc);
        await ProcessTimetableAsync(doc);
    }

    private async Task EnsureDataConstraintsAsync()
    {
        try
        {
            if (!await _context.Faculties.AnyAsync(f => f.Id == 1) && !_context.Faculties.Local.Any(f => f.Id == 1))
            {
                _context.Faculties.Add(new Faculty
                {
                    Id = 1,
                    Name = "Faculty 1",
                    Abbreviation = "FAC1"
                });
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception)
        {
            _context.ChangeTracker.Clear();
        }

        try
        {
            if (!await _context.Buildings.AnyAsync(b => b.Id == 1) && !_context.Buildings.Local.Any(b => b.Id == 1))
            {
                _context.Buildings.Add(new Building
                {
                    Id = 1,
                    Name = "Building 1",
                    Street = "Imported",
                    HouseNumber = "1",
                    City = "Imported",
                    PostalCode = "00-000",
                    FacultyId = 1
                });
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception)
        {
            _context.ChangeTracker.Clear();
        }

        try
        {
            if (!await _context.AcademicYears.AnyAsync(y => y.Id == 1) && !_context.AcademicYears.Local.Any(y => y.Id == 1))
            {
                _context.AcademicYears.Add(new AcademicYear
                {
                    Id = 1,
                    Name = "2025/2026",
                    StartDate = new DateOnly(2025, 10, 1),
                    EndDate = new DateOnly(2026, 9, 30)
                });
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception)
        {
            _context.ChangeTracker.Clear();
        }

        try
        {
            if (!await _context.Semesters.AnyAsync(s => s.Id == 1) && !_context.Semesters.Local.Any(s => s.Id == 1))
            {
                _context.Semesters.Add(new Semester
                {
                    Id = 1,
                    AcademicYearId = 1,
                    Name = "Semester 1",
                    StartDate = new DateOnly(2025, 10, 1),
                    EndDate = new DateOnly(2026, 2, 28)
                });
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception)
        {
            _context.ChangeTracker.Clear();
        }
    }

    private Dictionary<int, string> ParseAcademicTitles(XDocument doc)
    {
        var titles = new Dictionary<int, string>();
        var elements = doc.Descendants("tabela_tytuly");

        foreach (var el in elements)
        {
            var idText = el.Element("ID")?.Value;
            var nameText = el.Element("NAZWA")?.Value;

            if (string.IsNullOrWhiteSpace(idText) || !int.TryParse(idText, out int id)) continue;
            if (string.IsNullOrWhiteSpace(nameText)) continue;

            titles[id] = nameText;
        }

        return titles;
    }

    private async Task ProcessFieldsOfStudyAsync(XDocument doc)
    {
        var elements = doc.Descendants("tabela_studia");
        var existingIds = await _context.FieldsOfStudy.Select(f => f.Id).ToHashSetAsync();

        foreach (var el in elements)
        {
            var idText = el.Element("ID")?.Value;
            var nameText = el.Element("NAZWA")?.Value;

            if (string.IsNullOrWhiteSpace(idText) || !int.TryParse(idText, out int id)) continue;
            if (string.IsNullOrWhiteSpace(nameText)) continue;
            if (existingIds.Contains(id) || _context.FieldsOfStudy.Local.Any(f => f.Id == id)) continue;

            try
            {
                _context.FieldsOfStudy.Add(new FieldOfStudy
                {
                    Id = id,
                    FacultyId = 1,
                    Name = nameText,
                    Degree = "Imported",
                    Mode = StudyMode.FullTime
                });
                await _context.SaveChangesAsync();
                existingIds.Add(id);
            }
            catch (Exception)
            {
                _context.ChangeTracker.Clear();
            }
        }
    }

    private async Task ProcessSpecializationsAsync(XDocument doc)
    {
        var elements = doc.Descendants("tabela_specjalnosci");
        var existingIds = await _context.Specializations.Select(s => s.Id).ToHashSetAsync();

        foreach (var el in elements)
        {
            var idText = el.Element("ID")?.Value;
            var nameText = el.Element("NAZWA")?.Value;

            if (string.IsNullOrWhiteSpace(idText) || !int.TryParse(idText, out int id)) continue;
            if (string.IsNullOrWhiteSpace(nameText)) continue;
            if (existingIds.Contains(id) || _context.Specializations.Local.Any(s => s.Id == id)) continue;

            try
            {
                _context.Specializations.Add(new Specialization
                {
                    Id = id,
                    Name = nameText
                });
                await _context.SaveChangesAsync();
                existingIds.Add(id);
            }
            catch (Exception)
            {
                _context.ChangeTracker.Clear();
            }
        }
    }

    private async Task ProcessRoomsAsync(XDocument doc)
    {
        var elements = doc.Descendants("tabela_sale");
        var existingIds = await _context.Rooms.Select(r => r.Id).ToHashSetAsync();

        foreach (var el in elements)
        {
            var idText = el.Element("ID")?.Value;
            var nameText = el.Element("NAZWA")?.Value;

            if (string.IsNullOrWhiteSpace(idText) || !int.TryParse(idText, out int id)) continue;
            if (string.IsNullOrWhiteSpace(nameText)) continue;
            if (existingIds.Contains(id) || _context.Rooms.Local.Any(r => r.Id == id)) continue;

            try
            {
                _context.Rooms.Add(new Room
                {
                    Id = id,
                    RoomNumber = nameText,
                    BuildingId = 1,
                    RoomType = RoomType.Other
                });
                await _context.SaveChangesAsync();
                existingIds.Add(id);
            }
            catch (Exception)
            {
                _context.ChangeTracker.Clear();
            }
        }
    }

    private async Task ProcessTeachersAsync(XDocument doc, Dictionary<int, string> academicTitles)
    {
        var elements = doc.Descendants("tabela_nauczyciele");
        var existingIds = await _context.Teachers.Select(t => t.Id).ToHashSetAsync();

        foreach (var el in elements)
        {
            var idText = el.Element("ID")?.Value;
            var firstNameText = el.Element("IMIE")?.Value;
            var lastNameText = el.Element("NAZW")?.Value;
            var academicTitleText = el.Element("ID_TYT")?.Value;

            if (string.IsNullOrWhiteSpace(idText) || !int.TryParse(idText, out int id)) continue;
            if (string.IsNullOrWhiteSpace(firstNameText) || string.IsNullOrWhiteSpace(lastNameText)) continue;
            if (existingIds.Contains(id) || _context.Teachers.Local.Any(t => t.Id == id)) continue;

            string title = "None";
            if (!string.IsNullOrWhiteSpace(academicTitleText) && int.TryParse(academicTitleText, out int titleId))
            {
                if (academicTitles.TryGetValue(titleId, out var titleName))
                {
                    title = titleName;
                }
            }

            try
            {
                _context.Teachers.Add(new Teacher
                {
                    Id = id,
                    FirstName = firstNameText,
                    LastName = lastNameText,
                    AcademicTitle = title
                });
                await _context.SaveChangesAsync();
                existingIds.Add(id);
            }
            catch (Exception)
            {
                _context.ChangeTracker.Clear();
            }
        }
    }

    private async Task ProcessSubjectsAsync(XDocument doc)
    {
        var elements = doc.Descendants("tabela_przedmioty");
        var existingIds = await _context.Subjects.Select(s => s.Id).ToHashSetAsync();

        foreach (var el in elements)
        {
            var idText = el.Element("ID")?.Value;
            var nameText = el.Element("NAZWA")?.Value;
            var abbrText = el.Element("NAZ_SK")?.Value;

            if (string.IsNullOrWhiteSpace(idText) || !int.TryParse(idText, out int id)) continue;
            if (string.IsNullOrWhiteSpace(nameText)) continue;
            if (existingIds.Contains(id) || _context.Subjects.Local.Any(s => s.Id == id)) continue;

            string finalAbbr = string.IsNullOrWhiteSpace(abbrText) ? nameText : abbrText;
            if (finalAbbr.Length > 20)
            {
                finalAbbr = finalAbbr.Substring(0, 20);
            }

            try
            {
                _context.Subjects.Add(new Subject
                {
                    Id = id,
                    Name = nameText,
                    Abbreviation = finalAbbr,
                    Code = "IMPORTED"
                });
                await _context.SaveChangesAsync();
                existingIds.Add(id);
            }
            catch (Exception)
            {
                _context.ChangeTracker.Clear();
            }
        }
    }

    private async Task ProcessTimetableAsync(XDocument doc)
    {
        var rooms = await _context.Rooms.Select(r => r.Id).ToHashSetAsync();
        var teachers = await _context.Teachers.Select(t => t.Id).ToHashSetAsync();
        var subjects = await _context.Subjects.Select(s => s.Id).ToHashSetAsync();
        var fields = await _context.FieldsOfStudy.Select(f => f.Id).ToHashSetAsync();
        var semesters = await _context.Semesters.Select(s => s.Id).ToHashSetAsync();
        var specializations = await _context.Specializations.Select(s => s.Id).ToHashSetAsync();
        var groups = await _context.Groups.Select(g => g.Id).ToHashSetAsync();
        var timetables = await _context.Timetables
            .Select(t => new { t.SubjectId, t.TeacherId, t.RoomId, t.GroupId, t.ClassType, t.DayOfWeek, t.StartTime, t.EndTime, t.WeekCycle })
            .ToHashSetAsync();
        var elements = doc.Descendants("tabela_rozklad");

        foreach (var el in elements)
        {
            var prjIdText = el.Element("ID_PRZ")?.Value;
            var tchrIdText = el.Element("ID_NAUCZ")?.Value;
            var roomIdText = el.Element("ID_SALA")?.Value;
            var grpIdText = el.Element("GRUPA")?.Value;
            var dayText = el.Element("DZIEN")?.Value;
            var hourText = el.Element("GODZ")?.Value;
            var durationText = el.Element("ILOSC")?.Value;
            var tygText = el.Element("TYG")?.Value;
            var semText = el.Element("SEM")?.Value;
            var specIdText = el.Element("ID_SPEC")?.Value;
            var stIdText = el.Element("ID_ST")?.Value;

            if (!int.TryParse(prjIdText, out int prjId) ||
                !int.TryParse(tchrIdText, out int tchrId) ||
                !int.TryParse(roomIdText, out int roomId) ||
                !int.TryParse(grpIdText, out int groupId) ||
                !int.TryParse(dayText, out int dayNum) ||
                !int.TryParse(hourText, out int hourBlock) ||
                !int.TryParse(durationText, out int durationBlocks) ||
                !int.TryParse(tygText, out int tygNum) ||
                !int.TryParse(semText, out int semesterId) ||
                !int.TryParse(specIdText, out int specId) ||
                !int.TryParse(stIdText, out int fieldOfStudyId))
            {
                continue;
            }

            if (!rooms.Contains(roomId) || 
                !teachers.Contains(tchrId) || 
                !subjects.Contains(prjId) || 
                !fields.Contains(fieldOfStudyId))
            {
                continue;
            }

            if (!semesters.Contains(semesterId) && !_context.Semesters.Local.Any(s => s.Id == semesterId))
            {
                try
                {
                    _context.Semesters.Add(new Semester
                    {
                        Id = semesterId,
                        AcademicYearId = 1,
                        Name = $"Semester {semesterId}",
                        StartDate = new DateOnly(2025, 10, 1),
                        EndDate = new DateOnly(2026, 2, 28)
                    });
                    await _context.SaveChangesAsync();
                    semesters.Add(semesterId);
                }
                catch (Exception)
                {
                    _context.ChangeTracker.Clear();
                }
            }

            int? finalizedSpecializationId = null;
            if (specId > 0)
            {
                if (!specializations.Contains(specId)) continue;
                finalizedSpecializationId = specId;
            }

            if (!TimeBlocks.TryGetValue(hourBlock, out var startBlock) || 
                !TimeBlocks.TryGetValue(hourBlock + durationBlocks - 1, out var endBlock))
            {
                continue;
            }

            TimeSpan startTime = startBlock.Start;
            TimeSpan endTime = endBlock.End;
            DayOfWeek day = dayNum == 7 ? DayOfWeek.Sunday : (DayOfWeek)dayNum;
            WeekCycle weekCycle = tygNum switch
            {
                0 => WeekCycle.Weekly,
                1 => WeekCycle.Odd,
                2 => WeekCycle.Even,
                _ => WeekCycle.Weekly
            };

            string typeStr = el.Element("RODZ")?.Value?.ToLower() ?? "";
            ClassType classType = typeStr switch
            {
                "w" => ClassType.Lecture,
                "lab" => ClassType.Laboratory,
                "ps" => ClassType.SpecialisedLaboratory,
                "ćw" or "cw" => ClassType.Exercise,
                "sem" => ClassType.Seminar,
                "proj" => ClassType.Project,
                _ => ClassType.Lecture
            };

            if (!groups.Contains(groupId) && !_context.Groups.Local.Any(g => g.Id == groupId))
            {
                try
                {
                    _context.Groups.Add(new Group
                    {
                        Id = groupId,
                        Name = $"Group {groupId}",
                        ClassType = classType,
                        SemesterId = semesterId,
                        FieldOfStudyId = fieldOfStudyId,
                        SpecializationId = finalizedSpecializationId
                    });
                    await _context.SaveChangesAsync();
                    groups.Add(groupId);
                }
                catch (Exception)
                {
                    _context.ChangeTracker.Clear();
                }
            }

            var key = new 
            { 
                SubjectId = prjId, 
                TeacherId = tchrId, 
                RoomId = roomId, 
                GroupId = groupId, 
                ClassType = classType, 
                DayOfWeek = day, 
                StartTime = startTime, 
                EndTime = endTime, 
                WeekCycle = weekCycle 
            };

            if (timetables.Contains(key)) continue;

            try
            {
                _context.Timetables.Add(new Timetable
                {
                    SubjectId = prjId,
                    TeacherId = tchrId,
                    RoomId = roomId,
                    GroupId = groupId,
                    ClassType = classType,
                    DayOfWeek = day,
                    StartTime = startTime,
                    EndTime = endTime,
                    WeekCycle = weekCycle
                });
                await _context.SaveChangesAsync();
                timetables.Add(key);
            }
            catch (Exception)
            {
                _context.ChangeTracker.Clear();
            }
        }
    }
}