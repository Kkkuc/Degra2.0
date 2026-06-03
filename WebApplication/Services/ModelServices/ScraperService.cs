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
        { 12, (new TimeSpan(18, 25, 0), new TimeSpan(19, 10, 0)) }
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

        await EnsureDataConstraintsAsync();

        await ProcessRoomsAsync(doc);
        await ProcessTeachersAsync(doc);
        await ProcessSubjectsAsync(doc);
        await ProcessTimetableAsync(doc);
    }

    private async Task EnsureDataConstraintsAsync()
    {
        if (!await _context.Faculties.AnyAsync(f => f.Id == 1))
        {
            _context.Faculties.Add(new Faculty
            {
                Id = 1,
                Name = "unknown",
                Abbreviation = "unidentif"
            });
            await _context.SaveChangesAsync();
        }

        if (!await _context.Buildings.AnyAsync(b => b.Id == 1))
        {
            _context.Buildings.Add(new Building
            {
                Id = 1,
                Name = "unknown",
                Street = "unknown",
                HouseNumber = "unidentif",
                City = "unknown",
                PostalCode = "00-000",
                FacultyId = 1
            });
            await _context.SaveChangesAsync();
        }

        if (!await _context.AcademicYears.AnyAsync(y => y.Id == 1))
        {
            _context.AcademicYears.Add(new AcademicYear
            {
                Id = 1,
                Name = "unknown",
                StartDate = new DateOnly(2025, 10, 1),
                EndDate = new DateOnly(2026, 9, 30)
            });
            await _context.SaveChangesAsync();
        }

        if (!await _context.Semesters.AnyAsync(s => s.Id == 1))
        {
            _context.Semesters.Add(new Semester
            {
                Id = 1,
                AcademicYearId = 1,
                Name = "unknown",
                StartDate = new DateOnly(2025, 10, 1),
                EndDate = new DateOnly(2026, 2, 28)
            });
            await _context.SaveChangesAsync();
        }

        if (!await _context.FieldsOfStudy.AnyAsync(f => f.Id == 1))
        {
            _context.FieldsOfStudy.Add(new FieldOfStudy
            {
                Id = 1,
                FacultyId = 1,
                Name = "unknown",
                Degree = "unknown",
                Mode = StudyMode.FullTime
            });
            await _context.SaveChangesAsync();
        }
    }

    private async Task ProcessRoomsAsync(XDocument doc)
    {
        var elements = doc.Descendants("tabela_sale");
        foreach (var el in elements)
        {
            var idText = el.Element("ID")?.Value;
            var nameText = el.Element("NAZWA")?.Value;

            if (string.IsNullOrWhiteSpace(idText) || !int.TryParse(idText, out int id)) continue;
            if (await _context.Rooms.AnyAsync(r => r.Id == id)) continue;

            _context.Rooms.Add(new Room
            {
                Id = id,
                RoomNumber = string.IsNullOrWhiteSpace(nameText) ? "unknown" : nameText,
                BuildingId = 1,
                RoomType = RoomType.Other
            });
        }
        await _context.SaveChangesAsync();
    }

    private async Task ProcessTeachersAsync(XDocument doc)
    {
        var elements = doc.Descendants("tabela_nauczyciele");
        foreach (var el in elements)
        {
            var idText = el.Element("ID")?.Value;
            var firstNameText = el.Element("IMIE")?.Value;
            var lastNameText = el.Element("NAZW")?.Value;
            var academicTitleText = el.Element("ID_TYT")?.Value;

            if (string.IsNullOrWhiteSpace(idText) || !int.TryParse(idText, out int id)) continue;
            if (await _context.Teachers.AnyAsync(t => t.Id == id)) continue;

            _context.Teachers.Add(new Teacher
            {
                Id = id,
                FirstName = string.IsNullOrWhiteSpace(firstNameText) ? "unknown" : firstNameText,
                LastName = string.IsNullOrWhiteSpace(lastNameText) ? "unknown" : lastNameText,
                AcademicTitle = string.IsNullOrWhiteSpace(academicTitleText) ? "unknown" : academicTitleText
            });
        }
        await _context.SaveChangesAsync();
    }

    private async Task ProcessSubjectsAsync(XDocument doc)
    {
        var elements = doc.Descendants("tabela_przedmioty");
        foreach (var el in elements)
        {
            var idText = el.Element("ID")?.Value;
            var nameText = el.Element("NAZWA")?.Value;
            var abbrText = el.Element("NAZ_SK")?.Value;

            if (string.IsNullOrWhiteSpace(idText) || !int.TryParse(idText, out int id)) continue;
            if (await _context.Subjects.AnyAsync(s => s.Id == id)) continue;

            string finalName = string.IsNullOrWhiteSpace(nameText) ? "unknown" : nameText;
            string finalAbbr = string.IsNullOrWhiteSpace(abbrText) ? "unknown" : abbrText;

            _context.Subjects.Add(new Subject
            {
                Id = id,
                Name = finalName,
                Abbreviation = finalAbbr,
                Code = "IMPORTED"
            });
        }
        await _context.SaveChangesAsync();
    }

    private async Task ProcessTimetableAsync(XDocument doc)
    {
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

            if (!await _context.FieldsOfStudy.AnyAsync(f => f.Id == fieldOfStudyId))
            {
                _context.FieldsOfStudy.Add(new FieldOfStudy
                {
                    Id = fieldOfStudyId,
                    FacultyId = 1,
                    Name = "unknown",
                    Degree = "unknown",
                    Mode = StudyMode.FullTime
                });
                await _context.SaveChangesAsync();
            }

            if (!await _context.Semesters.AnyAsync(s => s.Id == semesterId))
            {
                _context.Semesters.Add(new Semester
                {
                    Id = semesterId,
                    AcademicYearId = 1,
                    Name = "unknown",
                    StartDate = new DateOnly(2025, 10, 1),
                    EndDate = new DateOnly(2026, 2, 28)
                });
                await _context.SaveChangesAsync();
            }

            int? finalizedSpecializationId = null;
            if (specId > 0)
            {
                if (!await _context.Specializations.AnyAsync(s => s.Id == specId))
                {
                    _context.Specializations.Add(new Specialization
                    {
                        Id = specId,
                        Name = "unknown"
                    });
                    await _context.SaveChangesAsync();
                }
                finalizedSpecializationId = specId;
            }

            if (!await _context.Groups.AnyAsync(g => g.Id == groupId))
            {
                _context.Groups.Add(new Group 
                { 
                    Id = groupId, 
                    Name = "unknown", 
                    ClassType = classType,
                    SemesterId = semesterId,
                    FieldOfStudyId = fieldOfStudyId,
                    SpecializationId = finalizedSpecializationId
                });
                await _context.SaveChangesAsync();
            }

            if (!await _context.Teachers.AnyAsync(t => t.Id == tchrId))
            {
                _context.Teachers.Add(new Teacher 
                { 
                    Id = tchrId, 
                    FirstName = "unknown", 
                    LastName = "unknown",
                    AcademicTitle = "unknown"
                });
                await _context.SaveChangesAsync();
            }

            if (!await _context.Rooms.AnyAsync(r => r.Id == roomId))
            {
                _context.Rooms.Add(new Room 
                { 
                    Id = roomId, 
                    RoomNumber = "unknown", 
                    BuildingId = 1, 
                    RoomType = RoomType.Other 
                });
                await _context.SaveChangesAsync();
            }

            if (!await _context.Subjects.AnyAsync(s => s.Id == prjId))
            {
                _context.Subjects.Add(new Subject 
                { 
                    Id = prjId, 
                    Name = "unknown", 
                    Abbreviation = "unknown", 
                    Code = "IMPORTED" 
                });
                await _context.SaveChangesAsync();
            }

            bool timetableExists = await _context.Timetables.AnyAsync(t =>
                t.SubjectId == prjId &&
                t.TeacherId == tchrId &&
                t.RoomId == roomId &&
                t.GroupId == groupId &&
                t.ClassType == classType &&
                t.DayOfWeek == day &&
                t.StartTime == startTime &&
                t.EndTime == endTime &&
                t.WeekCycle == weekCycle);

            if (!timetableExists)
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
            }
        }
        await _context.SaveChangesAsync();
    }
}