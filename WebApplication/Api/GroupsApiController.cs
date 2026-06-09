using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.DTOs.Group;
using WebApplication.Services.Interfaces;

namespace WebApplication.Api;

[ApiController]
[Authorize(Roles = "Moderator")]
[Route("api/groups")]
public class GroupsApiController(
    IGroupsService groupsService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<GroupAdminItemDto>>> GetGroups(
        [FromQuery] string? name,
        [FromQuery] int? semesterId,
        [FromQuery] int? fieldOfStudyId,
        [FromQuery] int? specializationId,
        [FromQuery] int? classType)
    {
        var filter = new GroupFilterDto
        {
            Name = name,
            SemesterId = semesterId,
            FieldOfStudyId = fieldOfStudyId,
            SpecializationId = specializationId,
            ClassType = classType
        };

        var groups =
            await groupsService.GetAllForAdminAsync(filter);

        return Ok(groups);
    }

    [HttpGet("metadata")]
    public async Task<ActionResult<GroupAdminMetadataDto>>
        GetMetadata()
    {
        return Ok(await groupsService.GetAdminMetadataAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GroupFormDto>> GetGroup(
        int id)
    {
        var group = await groupsService.GetFormByIdAsync(id);

        return group is null
            ? NotFound()
            : Ok(group);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGroup(
        [FromBody] GroupFormDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (!await groupsService.IsValidForeignKeysAsync(dto))
        {
            return BadRequest(new
            {
                message =
                    "Wybrano nieprawidłowy semestr, kierunek, " +
                    "specjalizację lub typ zajęć."
            });
        }

        await groupsService.CreateAsync(dto);

        return StatusCode(
            StatusCodes.Status201Created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateGroup(
        int id,
        [FromBody] GroupFormDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest(new
            {
                message =
                    "Identyfikator w adresie nie zgadza się " +
                    "z identyfikatorem obiektu."
            });
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (!await groupsService.IsValidForeignKeysAsync(dto))
        {
            return BadRequest(new
            {
                message =
                    "Wybrano nieprawidłowy semestr, kierunek, " +
                    "specjalizację lub typ zajęć."
            });
        }

        var updated =
            await groupsService.UpdateAsync(dto);

        return updated
            ? NoContent()
            : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteGroup(int id)
    {
        try
        {
            var deleted =
                await groupsService.DeleteAsync(id);

            return deleted
                ? NoContent()
                : NotFound();
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message =
                    "Nie można usunąć grupy, ponieważ jest " +
                    "używana w planie zajęć lub ma przypisanych studentów."
            });
        }
    }
}