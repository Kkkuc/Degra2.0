using WebApplication.Models.enums;

namespace WebApplication.DTOs.Timetable;

public class TimetableCreateDto
{
    public int SubjectId { get; set; }
    public int TeacherId { get; set; }
    public int RoomId { get; set; }
    public int GroupId { get; set; }
    public ClassType ClassType { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public WeekCycle WeekCycle { get; set; }
}