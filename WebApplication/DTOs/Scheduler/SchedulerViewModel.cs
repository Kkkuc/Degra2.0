using WebApplication.DTOs.Subject;
using WebApplication.DTOs.Timetable;

namespace WebApplication.DTOs.Scheduler;

public class SchedulerViewModel
{
    public List<TimetableEntryDto> Lessons { get; set; } = [];
    public List<SubjectDto> Subjects { get; set; } = [];
    public List<string> TimeSlots { get; set; } = [
        "08:30 – 09:15", "09:15 – 10:00", "10:15 – 11:00", "11:00 – 11:45", 
        "12:00 – 12:45", "12:45 – 13:30", "14:00 – 14:45", "14:45 – 15:30", 
        "16:00 – 16:45", "16:45 – 17:30", "17:40 – 18:25", "18:25 – 19:10"
    ];
}