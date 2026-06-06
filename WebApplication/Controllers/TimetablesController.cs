using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.DTOs.Timetable;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimetablesController(ITimetablesService timetablesService) : ControllerBase
{
    // GET: Timetables
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TimetableListDto>>> Index()
    {
        var data = await timetablesService.GetAllWithRelationsAsync();
        return Ok(data);
    }

    [HttpPost("filter")]
    public async Task<ActionResult<IEnumerable<TimetableListDto>>> Filter([FromBody] TimetableFilterDto filter)
    {
        var data = await timetablesService.GetFilteredAsync(filter);
        return Ok(data);
    }
    
    // GET: Timetables/Details/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TimetableDetailsDto>> Details(int id)
    {
        var timetable = await timetablesService.GetByIdWithRelationsAsync(id);
        if (timetable == null)
        {
            return NotFound(new { Message = $"Timetable with ID {id} not found." });
        }

        return Ok(timetable);
    }

    // GET: Timetables/Create
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TimetableCreateDto timetableDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await timetablesService.CreateAsync(timetableDto);
        return StatusCode(StatusCodes.Status201Created, new { Message = "Timetable created successfully." });
    }

    // POST: Timetables/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

    // GET: Timetables/Edit/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Edit(int id, [FromBody] TimetableEditDto timetableDto)
    {
        if (id != timetableDto.Id)
        {
            return BadRequest(new { Message = "ID mismatch." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await timetablesService.UpdateAsync(timetableDto);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await timetablesService.ExistsAsync(timetableDto.Id))
            {
                return NotFound(new { Message = "Timetable not found." });
            }
            throw;
        }

        return NoContent();
    }

    // POST: Timetables/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

    // GET: Timetables/Delete/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await timetablesService.ExistsAsync(id))
        {
            return NotFound(new { Message = "Timetable not found." });
        }

        await timetablesService.DeleteAsync(id);
        return NoContent();
    }
}
