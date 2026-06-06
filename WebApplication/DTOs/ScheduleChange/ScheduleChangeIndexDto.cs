namespace WebApplication.DTOs.ScheduleChange;

public record ScheduleChangeIndexDto(
    int Id,
    string OriginalEntryText,
    DateTime? ChangeDate,
    string NewRoom);