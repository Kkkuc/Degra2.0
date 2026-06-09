using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.DTOs.Scheduler;
using WebApplication.DTOs.Subject;
using WebApplication.DTOs.Timetable;
using WebApplication.Models.enums;
using WebApplication.Models.SchedulerSlots;
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
        var lessonsDto = new List<TimetableEntryDto>();

        foreach (var lesson in rawLessons)
        {
            var startSlot =
                ScheduleTimeSlots.FindIndexByStartTime(lesson.StartTime);

            var duration =
                ScheduleTimeSlots.CalculateDurationInSlots(
                    lesson.StartTime,
                    lesson.EndTime);

            if (!startSlot.HasValue || !duration.HasValue)
            {
                continue;
            }

            lessonsDto.Add(new TimetableEntryDto
            {
                Id = lesson.Id.ToString(),
                SubjectId = lesson.SubjectId,
                Subject = lesson.SubjectName,
                Color = GetColorForSubject(lesson.SubjectId),
                Day = (int)lesson.DayOfWeek - 1,
                StartSlot = startSlot.Value,
                Duration = duration.Value,
                Room = lesson.RoomNumber,
                Teacher = lesson.TeacherFullName,
                Time = $@"{lesson.StartTime:hh\:mm} – {lesson.EndTime:hh\:mm}"
            });
        }

        var subjectsDto = rawSubjects.Select(s => new SubjectIndexDto
        (
            s.Id,
            s.Name
        )).ToList();

        return new SchedulerViewModel
        {
            Lessons = lessonsDto,
            Subjects = subjectsDto,
            TimeSlots = ScheduleTimeSlots.All.ToList()
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

    private static readonly string[] SubjectColors =
    [
        "#EF4444",
        "#F97316",
        "#22C55E",
        "#3B82F6",
        "#8B5CF6",
        "#EC4899"
    ];

    private static string GetColorForSubject(int id)
    {
        return SubjectColors[Math.Abs(id) % SubjectColors.Length];
    }

   
}