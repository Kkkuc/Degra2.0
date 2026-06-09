using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.DTOs.Student;
using WebApplication.Services.Interfaces;

namespace WebApplication.Api;

[ApiController]
[Authorize(Roles = "Moderator")]
[Route("api/students")]
public class StudentsApiController(
    IStudentsService studentsService)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<StudentAdminItemDto>>>
        GetAll(
            [FromQuery] string? search,
            [FromQuery] int? groupId)
    {
        var filter = new StudentAdminFilterDto
        {
            Search = search,
            GroupId = groupId
        };

        return Ok(
            await studentsService.GetAllForAdminAsync(filter));
    }

    [HttpGet("metadata")]
    public async Task<ActionResult<StudentAdminMetadataDto>>
        GetMetadata()
    {
        return Ok(
            await studentsService.GetAdminMetadataAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StudentAdminDetailsDto>>
        GetById(int id)
    {
        var student =
            await studentsService.GetAdminDetailsAsync(id);

        return student is null
            ? NotFound()
            : Ok(student);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] StudentAdminFormDto dto)
    {
        NormalizeGroups(dto);

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (await studentsService
                .StudentNumberExistsAsync(dto.StudentId))
        {
            return Conflict(new
            {
                message =
                    "Student o takim numerze albumu już istnieje."
            });
        }

        if (!await studentsService
                .GroupsExistAsync(dto.GroupIds))
        {
            return BadRequest(new
            {
                message =
                    "Co najmniej jedna z wybranych grup nie istnieje."
            });
        }

        await studentsService.CreateForAdminAsync(dto);

        return StatusCode(
            StatusCodes.Status201Created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] StudentAdminFormDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest(new
            {
                message =
                    "Identyfikator w adresie nie zgadza się " +
                    "z identyfikatorem studenta."
            });
        }

        NormalizeGroups(dto);

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (await studentsService.StudentNumberExistsAsync(
                dto.StudentId,
                dto.Id))
        {
            return Conflict(new
            {
                message =
                    "Student o takim numerze albumu już istnieje."
            });
        }

        if (!await studentsService
                .GroupsExistAsync(dto.GroupIds))
        {
            return BadRequest(new
            {
                message =
                    "Co najmniej jedna z wybranych grup nie istnieje."
            });
        }

        var updated =
            await studentsService.UpdateForAdminAsync(dto);

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
                await studentsService.DeleteAsync(id);

            return deleted
                ? NoContent()
                : NotFound();
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message =
                    "Nie można usunąć studenta ze względu " +
                    "na inne powiązane dane."
            });
        }
    }

    private static void NormalizeGroups(
        StudentAdminFormDto dto)
    {
        dto.GroupIds = dto.GroupIds
            .Distinct()
            .ToList();
    }
}