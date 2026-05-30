namespace WebApplication.DTOs.Scheduler;

public record SchedulerFilterDto(
    int? FieldId,
    int? SemId,
    string? Gw, string? Gc, string? Gl, 
    string? Gps, string? Gp, string? Gs);