using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.DTOs.Scheduler;
using WebApplication.DTOs.Subject;
using WebApplication.DTOs.Timetable;
using WebApplication.Models.enums;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services.ModelServices;

public class SchedulerService(AppDbContext context) : ISchedulerService
{
    public async Task<SchedulerViewModel> GetSchedulerDataAsync(SchedulerFilterDto filter)
    {
        var query = context.Timetables.AsQueryable();

        // 1. Filtrowanie po kierunku i semestrze
        if (filter.FieldId.HasValue)
            query = query.Where(t => t.Group != null && t.Group.FieldOfStudyId == filter.FieldId);
        if (filter.SemId.HasValue) query = query.Where(t => t.Group != null && t.Group.SemesterId == filter.SemId);

        // 2. Filtrowanie po grupach dziekańskich i typach zajęć

        if (!string.IsNullOrEmpty(filter.Gw) || !string.IsNullOrEmpty(filter.Gc) ||
            !string.IsNullOrEmpty(filter.Gl) || !string.IsNullOrEmpty(filter.Gps) ||
            !string.IsNullOrEmpty(filter.Gp) || !string.IsNullOrEmpty(filter.Gs))
        {
            query = query.Where(t => t.Group != null && (
                (!string.IsNullOrEmpty(filter.Gw) && t.Group.ClassType == ClassType.Lecture && t.Group.Name.Contains(filter.Gw)) ||
                (!string.IsNullOrEmpty(filter.Gc) && t.Group.ClassType == ClassType.Exercise && t.Group.Name.Contains(filter.Gc)) ||
                (!string.IsNullOrEmpty(filter.Gl) && t.Group.ClassType == ClassType.Laboratory && t.Group.Name.Contains(filter.Gl)) ||
                (!string.IsNullOrEmpty(filter.Gps) && t.Group.ClassType == ClassType.SpecialisedLaboratory && t.Group.Name.Contains(filter.Gps)) ||
                (!string.IsNullOrEmpty(filter.Gp) && t.Group.ClassType == ClassType.Project && t.Group.Name.Contains(filter.Gp)) ||
                (!string.IsNullOrEmpty(filter.Gs) && t.Group.ClassType == ClassType.Seminar && t.Group.Name.Contains(filter.Gs))
            ));
        }

        // 3. Projekcja i pobranie zoptymalizowanych surowych danych
        var rawLessons = await query
            .Select(t => new
            {
                t.Id,
                t.SubjectId,
                SubjectName = t.Subject != null ? t.Subject.Name : "Brak",
                t.DayOfWeek,
                t.StartTime,
                t.EndTime,
                RoomNumber = t.Room != null ? t.Room.RoomNumber : "Brak",
                TeacherFullName = t.Teacher != null ? t.Teacher.FirstName + " " + t.Teacher.LastName : "Brak"
            })
            .ToListAsync();

        var rawSubjects = await query
            .Where(t => t.Subject != null)
            .Select(t => new { t.Subject!.Id, t.Subject.Name })
            .Distinct() // Usuwa duplikaty przedmiotów
            .ToListAsync();

        // 4. Mapowanie w pamięci RAM (przeliczenia slotów i kolorów)
        var lessonsDto = rawLessons.Select(t => new TimetableEntryDto
        {
            Id = t.Id.ToString(),
            SubjectId = t.SubjectId,
            Subject = t.SubjectName,
            Color = GetColorForSubject(t.SubjectId),
            Day = (int)t.DayOfWeek - 1,
            StartSlot = GetSlotIndex(t.StartTime),
            Duration = (int)((t.EndTime - t.StartTime).TotalMinutes / 45),
            Room = t.RoomNumber,
            Teacher = t.TeacherFullName,
            Time = $@"{t.StartTime:hh\:mm} – {t.EndTime:hh\:mm}"
        }).ToList();
        
        var rand = new Random();
        
        var subjectsDto = rawSubjects.Select(s => new SubjectIndexDto
        (
            s.Id,
            s.Name
        )).ToList();

        return new SchedulerViewModel
        {
            Lessons = lessonsDto,
            Subjects = subjectsDto
        };
    }

    public async Task<Dictionary<int, string>> GetFieldsOfStudyDropdownAsync()
    {
        return await context.FieldsOfStudy.ToDictionaryAsync(f => f.Id, f => f.Name);
    }

    public async Task<Dictionary<int, string>> GetSemestersDropdownAsync()
    {
        return await context.Semesters.ToDictionaryAsync(s => s.Id, s => s.Name);
    }

    // --- PRYWATNE METODY LOGIKI BIZNESOWEJ ---
    private static string GetColorForSubject(int id) =>
        (new[] { "#EF4444", "#F97316", "#22C55E", "#3B82F6", "#8B5CF6", "#EC4899" })[id % 6];

    private static int GetSlotIndex(TimeSpan time)
    {
        if (time >= new TimeSpan(20, 5, 0)) return 13;   
        if (time >= new TimeSpan(19, 20, 0)) return 12;
        if (time >= new TimeSpan(18, 25, 0)) return 11;
        if (time >= new TimeSpan(17, 40, 0)) return 10;
        if (time >= new TimeSpan(16, 45, 0)) return 9;
        if (time >= new TimeSpan(16, 0, 0)) return 8;
        if (time >= new TimeSpan(14, 45, 0)) return 7;
        if (time >= new TimeSpan(14, 0, 0)) return 6;
        if (time >= new TimeSpan(12, 45, 0)) return 5;
        if (time >= new TimeSpan(12, 0, 0)) return 4;
        if (time >= new TimeSpan(11, 0, 0)) return 3;
        if (time >= new TimeSpan(10, 15, 0)) return 2;
        if (time >= new TimeSpan(9, 15, 0)) return 1;
        return 0;
    }
}