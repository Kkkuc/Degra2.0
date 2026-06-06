using WebApplication.Models.enums;

namespace WebApplication.DTOs.Timetable;

public class TimetableDetailsDto
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public int TeacherId { get; set; }
    public int RoomId { get; set; }
    public int GroupId { get; set; }
    public ClassType ClassType { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public WeekCycle WeekCycle { get; set; }
    
    public string GroupName { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
}