namespace WebApplication.DTOs.Scheduler;

public sealed record ScheduleTimeSlotDto(
    int Index,
    TimeSpan StartTime,
    TimeSpan EndTime)
{
    public string DisplayTime =>
        $@"{StartTime:hh\:mm} – {EndTime:hh\:mm}";
}