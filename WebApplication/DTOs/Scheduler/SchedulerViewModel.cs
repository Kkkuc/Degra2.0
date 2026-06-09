using WebApplication.DTOs.Subject;
using WebApplication.DTOs.Timetable;

namespace WebApplication.DTOs.Scheduler;

public class SchedulerViewModel
{
    public List<TimetableEntryDto> Lessons { get; set; } = [];

    public List<SubjectIndexDto> Subjects { get; set; } = [];

    public List<ScheduleTimeSlotDto> TimeSlots { get; set; } = [];
}