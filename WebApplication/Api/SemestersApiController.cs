using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.DTOs.Semester;
using WebApplication.Services.Interfaces;

namespace WebApplication.Api;

[ApiController]
[Authorize(Roles = "Moderator")]
[Route("api/semesters")]
public class SemestersApiController(
    ISemestersService semestersService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<SemesterAdminItemDto>>>
        GetAll(
            [FromQuery] string? name,
            [FromQuery] int? academicYearId)
    {
        var filter = new SemesterFilterDto
        {
            Name = name,
            AcademicYearId = academicYearId
        };

        return Ok(
            await semestersService.GetAllForAdminAsync(filter));
    }

    [HttpGet("metadata")]
    public async Task<ActionResult<SemesterAdminMetadataDto>>
        GetMetadata()
    {
        return Ok(
            await semestersService.GetAdminMetadataAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SemesterFormDto>> GetById(int id)
    {
        var semester =
            await semestersService.GetFormByIdAsync(id);

        return semester is null
            ? NotFound()
            : Ok(semester);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] SemesterFormDto dto)
    {
        ValidateSemester(dto);

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (!await semestersService
                .AcademicYearExistsAsync(dto.AcademicYearId))
        {
            return BadRequest(new
            {
                message = "Wybrany rok akademicki nie istnieje."
            });
        }

        await semestersService.CreateAsync(dto);

        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] SemesterFormDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest(new
            {
                message =
                    "Identyfikator w adresie nie zgadza się " +
                    "z identyfikatorem semestru."
            });
        }

        ValidateSemester(dto);

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (!await semestersService
                .AcademicYearExistsAsync(dto.AcademicYearId))
        {
            return BadRequest(new
            {
                message = "Wybrany rok akademicki nie istnieje."
            });
        }

        var updated =
            await semestersService.UpdateAsync(dto);

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
                await semestersService.DeleteAsync(id);

            return deleted
                ? NoContent()
                : NotFound();
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message =
                    "Nie można usunąć semestru, ponieważ są " +
                    "do niego przypisane grupy lub inne dane."
            });
        }
    }

    private void ValidateSemester(SemesterFormDto dto)
    {
        if (dto.EndDate <= dto.StartDate)
        {
            ModelState.AddModelError(
                nameof(dto.EndDate),
                "Data zakończenia musi być późniejsza " +
                "od daty rozpoczęcia.");
        }
    }
}