using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication.DTOs.Faculty;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

[ApiController]
[Authorize(Roles = "Moderator")]
[Route("api/faculties")]
public class FacultiesApiController(IFacultiesService facultiesService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search)
    {
        var data = await facultiesService.GetAllAsync();
        if (!string.IsNullOrWhiteSpace(search))
        {
            data = data.Where(f => 
                f.Name.Contains(search, StringComparison.OrdinalIgnoreCase) || 
                f.Abbreviation.Contains(search, StringComparison.OrdinalIgnoreCase));
        }
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await facultiesService.GetByIdAsync(id);
        return dto == null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FacultyDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        await facultiesService.CreateAsync(dto);
        return StatusCode(201);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] FacultyDto dto)
    {
        if (id != dto.Id || !ModelState.IsValid) return BadRequest();
        var success = await facultiesService.UpdateAsync(dto);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await facultiesService.DeleteAsync(id);
        return NoContent();
    }
}