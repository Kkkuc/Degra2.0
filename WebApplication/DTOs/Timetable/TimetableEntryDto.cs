namespace WebApplication.DTOs.Timetable;

public class TimetableEntryDto
{
    public string Id { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Day { get; set; }
    public int StartSlot { get; set; }
    public int Duration { get; set; }
    public string Room { get; set; } = string.Empty;
    public string Teacher { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
}