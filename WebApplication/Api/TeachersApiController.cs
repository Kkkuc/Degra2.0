using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.DTOs.Teacher;
using WebApplication.Services.Interfaces;

namespace WebApplication.Api;

[ApiController]
[Authorize(Roles = "Moderator")]
[Route("api/admin/teachers")]
public class TeachersApiController(
    ITeachersService teachersService)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TeacherAdminItemDto>>>
        GetAll([FromQuery] string? search)
    {
        var filter = new TeacherAdminFilterDto
        {
            Search = search
        };

        return Ok(
            await teachersService.GetAllForAdminAsync(filter));
    }

    [HttpGet("metadata")]
    public async Task<ActionResult<TeacherAdminMetadataDto>>
        GetMetadata()
    {
        return Ok(
            await teachersService.GetAdminMetadataAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TeacherFormDto>>
        GetById(int id)
    {
        var teacher =
            await teachersService.GetFormByIdAsync(id);

        return teacher is null
            ? NotFound()
            : Ok(teacher);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] TeacherFormDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (
            !string.IsNullOrWhiteSpace(dto.Email) &&
            await teachersService.EmailExistsAsync(dto.Email)
        )
        {
            return Conflict(new
            {
                message =
                    "Nauczyciel o takim adresie e-mail już istnieje."
            });
        }

        await teachersService.CreateAsync(dto);

        return StatusCode(
            StatusCodes.Status201Created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] TeacherFormDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest(new
            {
                message =
                    "Identyfikator w adresie nie zgadza się " +
                    "z identyfikatorem nauczyciela."
            });
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (
            !string.IsNullOrWhiteSpace(dto.Email) &&
            await teachersService.EmailExistsAsync(
                dto.Email,
                dto.Id)
        )
        {
            return Conflict(new
            {
                message =
                    "Nauczyciel o takim adresie e-mail już istnieje."
            });
        }

        var updated =
            await teachersService.UpdateAsync(dto);

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
                await teachersService.DeleteAsync(id);

            return deleted
                ? NoContent()
                : NotFound();
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message =
                    "Nie można usunąć nauczyciela, ponieważ " +
                    "jest używany w planie zajęć lub ma konto."
            });
        }
    }
}