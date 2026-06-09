using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication.DTOs.Timetable;
using WebApplication.Services.Interfaces;

namespace WebApplication.Api;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Moderator")]
public class TimetablesController(ITimetablesService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TimetableListDto>>> GetAll() 
        => Ok(await service.GetAllWithRelationsAsync());

    [HttpPost("filter")]
    public async Task<ActionResult<IEnumerable<TimetableListDto>>> Filter([FromBody] TimetableFilterDto filter) 
        => Ok(await service.GetFilteredAsync(filter));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TimetableDetailsDto>> GetById(int id)
    {
        var result = await service.GetByIdWithRelationsAsync(id);
        return result != null ? Ok(result) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create(TimetableCreateDto dto)
    {
        await service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new {}, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TimetableEditDto dto)
    {
        if (id != dto.Id) return BadRequest("ID mismatch");
        
        if (!await service.ExistsAsync(id)) return NotFound();
        
        await service.UpdateAsync(dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await service.ExistsAsync(id)) return NotFound();
        
        await service.DeleteAsync(id);
        return NoContent();
    }
}