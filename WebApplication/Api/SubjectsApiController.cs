using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.DTOs.Subject;
using WebApplication.Services.Interfaces;

namespace WebApplication.Api;

[ApiController]
[Authorize(Roles = "Moderator")]
[Route("api/subjects")]
public class SubjectsApiController(
    ISubjectsService subjectsService)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<SubjectAdminItemDto>>>
        GetAll([FromQuery] string? search)
    {
        var filter = new SubjectAdminFilterDto
        {
            Search = search
        };

        return Ok(
            await subjectsService.GetAllForAdminAsync(filter));
    }

    [HttpGet("metadata")]
    public async Task<ActionResult<SubjectAdminMetadataDto>>
        GetMetadata()
    {
        return Ok(
            await subjectsService.GetAdminMetadataAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SubjectFormDto>>
        GetById(int id)
    {
        var subject =
            await subjectsService.GetFormByIdAsync(id);

        return subject is null
            ? NotFound()
            : Ok(subject);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] SubjectFormDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (await subjectsService.NameOrCodeExistsAsync(
                dto.Name,
                dto.Code))
        {
            return Conflict(new
            {
                message =
                    "Przedmiot o takiej nazwie lub kodzie już istnieje."
            });
        }

        await subjectsService.CreateAsync(dto);

        return StatusCode(
            StatusCodes.Status201Created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] SubjectFormDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest(new
            {
                message =
                    "Identyfikator w adresie nie zgadza się " +
                    "z identyfikatorem przedmiotu."
            });
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (await subjectsService.NameOrCodeExistsAsync(
                dto.Name,
                dto.Code,
                dto.Id))
        {
            return Conflict(new
            {
                message =
                    "Przedmiot o takiej nazwie lub kodzie już istnieje."
            });
        }

        var updated =
            await subjectsService.UpdateAsync(dto);

        return updated
            ? NoContent()
            : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted =
                await subjectsService.DeleteAsync(id);

            return deleted
                ? NoContent()
                : NotFound();
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message =
                    "Nie można usunąć przedmiotu, ponieważ jest " +
                    "używany w planie zajęć."
            });
        }
    }
}