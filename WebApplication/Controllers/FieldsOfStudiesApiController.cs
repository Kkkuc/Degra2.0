using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication.DTOs.FieldOfStudy;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Moderator")]
public class FieldsOfStudiesApiController(IFieldsOfStudiesService fieldsService) : ControllerBase
{
    // GET: api/FieldsOfStudiesApi/metadata
    // Służy do zasilenia <select> i <datalist> w modalach
    [HttpGet("metadata")]
    public async Task<IActionResult> GetMetadata()
    {
        var faculties = await fieldsService.GetFacultyDropdownListAsync();
        return Ok(new { faculties });
    }

    // GET: api/FieldsOfStudiesApi
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FieldOfStudyIndexDto>>> GetAll()
    {
        return Ok(await fieldsService.GetAllForIndexAsync());
    }

    // POST: api/FieldsOfStudiesApi/filter
    // Wzorowane na Timetable, jeśli planujesz rozbudować filtrowanie w przyszłości
    [HttpPost("filter")]
    public async Task<ActionResult<IEnumerable<FieldOfStudyIndexDto>>> Filter([FromBody] FieldOfStudyFilterDto filter)
    {
        // Tutaj w serwisie musiałbyś dodać metodę GetFilteredAsync(filter)
        return Ok(await fieldsService.GetFilteredAsync(filter));
    }

    // GET: api/FieldsOfStudiesApi/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<FieldOfStudyDetailsDto>> GetDetails(int id)
    {
        var dto = await fieldsService.GetDetailsByIdAsync(id);
        return dto != null ? Ok(dto) : NotFound();
    }

    // POST: api/FieldsOfStudiesApi
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FieldOfStudyFormDto dto)
    {
        await fieldsService.CreateAsync(dto);
        return StatusCode(StatusCodes.Status201Created);
    }

    // PUT: api/FieldsOfStudiesApi/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] FieldOfStudyFormDto dto)
    {
        if (id != dto.Id) return BadRequest("ID mismatch.");
        
        var success = await fieldsService.UpdateAsync(dto);
        return success ? NoContent() : NotFound();
    }

    // DELETE: api/FieldsOfStudiesApi/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await fieldsService.ExistsAsync(id)) return NotFound();
        
        await fieldsService.DeleteAsync(id);
        return NoContent();
    }
    
    [HttpGet("suggestions")]
    public async Task<ActionResult<IEnumerable<string>>> GetSuggestions()
    {
        var names = await fieldsService.GetUniqueNamesAsync();
        return Ok(names);
    }
}