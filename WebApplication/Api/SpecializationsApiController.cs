using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.DTOs.Specialization;
using WebApplication.Services.Interfaces;

namespace WebApplication.Api;

[ApiController]
[Authorize(Roles = "Moderator")]
[Route("api/specializations")]
public class SpecializationsApiController(
    ISpecializationsService specializationsService)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<SpecializationDto>>>
        GetAll([FromQuery] string? name)
    {
        var filter = new SpecializationFilterDto
        {
            Name = name
        };

        return Ok(
            await specializationsService.GetAllAsync(filter));
    }

    [HttpGet("metadata")]
    public async Task<ActionResult<SpecializationMetadataDto>>
        GetMetadata()
    {
        return Ok(
            await specializationsService.GetMetadataAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SpecializationDto>>
        GetById(int id)
    {
        var specialization =
            await specializationsService.GetByIdAsync(id);

        return specialization is null
            ? NotFound()
            : Ok(specialization);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] SpecializationDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (await specializationsService
                .NameExistsAsync(dto.Name))
        {
            return Conflict(new
            {
                message =
                    "Specjalizacja o takiej nazwie już istnieje."
            });
        }

        await specializationsService.CreateAsync(dto);

        return StatusCode(
            StatusCodes.Status201Created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] SpecializationDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest(new
            {
                message =
                    "Identyfikator w adresie nie zgadza się " +
                    "z identyfikatorem specjalizacji."
            });
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (await specializationsService.NameExistsAsync(
                dto.Name,
                dto.Id))
        {
            return Conflict(new
            {
                message =
                    "Specjalizacja o takiej nazwie już istnieje."
            });
        }

        var updated =
            await specializationsService.UpdateAsync(dto);

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
                await specializationsService.DeleteAsync(id);

            return deleted
                ? NoContent()
                : NotFound();
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message =
                    "Nie można usunąć specjalizacji, ponieważ " +
                    "jest przypisana do jednej lub kilku grup."
            });
        }
    }
}