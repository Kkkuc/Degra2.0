using Microsoft.AspNetCore.Mvc;

namespace WebApplication.DTOs.Scheduler;

public record SchedulerFilterDto(
    [FromQuery(Name = "fieldId")] int? FieldId,
    [FromQuery(Name = "semId")] int? SemId,
    [FromQuery(Name = "gW")] string? Gw, 
    [FromQuery(Name = "gC")] string? Gc, 
    [FromQuery(Name = "gL")] string? Gl, 
    [FromQuery(Name = "gPs")] string? Gps, 
    [FromQuery(Name = "gP")] string? Gp, 
    [FromQuery(Name = "gS")] string? Gs
);