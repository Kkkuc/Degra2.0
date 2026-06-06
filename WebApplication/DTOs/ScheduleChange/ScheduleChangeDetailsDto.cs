namespace WebApplication.DTOs.ScheduleChange;

public record ScheduleChangeDetailsDto(
    int Id,
    DateTime? ChangeDate,
    string OriginalEntryText, // "Przedmiot | Nauczyciel | Dzień Start"
    string NewTeacher,
    string NewRoom,
    TimeSpan? NewStartTime,
    TimeSpan? NewEndTime);