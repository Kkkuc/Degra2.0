using Microsoft.AspNetCore.Mvc;
using WebApplication.DTOs.Scheduler;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

public class SchedulerController(ISchedulerService schedulerService) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpGet("api/scheduler")]
    [Produces("application/json")]
    public async Task<IActionResult> GetScheduler([FromQuery] SchedulerFilterDto filter)
    {
        try
        {
            var data = await schedulerService.GetSchedulerDataAsync(filter);
            return Ok(new SchedulerResponseDto
            {
                Lessons = data.Lessons,
                Subjects = data.Subjects,
                TimeSlots = data.TimeSlots
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Błąd pobierania planu.", Details = ex.Message });
        }
    }
    
    [HttpGet("api/scheduler/filters")]
    [Produces("application/json")]
    public async Task<IActionResult> GetFiltersDropdowns()
    {
        var fields = await schedulerService.GetFieldsOfStudyDropdownAsync();
        var semesters = await schedulerService.GetSemestersDropdownAsync();

        return Ok(new SchedulerFiltersDropdownDto
        {
            FieldsOfStudy = fields.Select(f => new DropdownItemDto(f.Key, f.Value)).ToList(),
            Semesters = semesters.Select(s => new DropdownItemDto(s.Key, s.Value)).ToList()
        });
    }
}