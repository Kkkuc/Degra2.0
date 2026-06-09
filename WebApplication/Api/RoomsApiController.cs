using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.DTOs.Room;
using WebApplication.Services.Interfaces;

namespace WebApplication.Api;

[ApiController]
[Authorize(Roles = "Moderator")]
[Route("api/admin/rooms")]
public class RoomsApiController(
    IRoomsService roomsService)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<RoomAdminItemDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] int? roomId,
        [FromQuery] int? buildingId)
    {
        var filter = new RoomAdminFilterDto
        {
            Search = search,
            RoomId = roomId,
            BuildingId = buildingId
        };

        return Ok(
            await roomsService.GetAllForAdminAsync(filter));
    }

    [HttpGet("metadata")]
    public async Task<ActionResult<RoomAdminMetadataDto>> GetMetadata()
    {
        return Ok(
            await roomsService.GetAdminMetadataAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RoomFormDto>> GetById(int id)
    {
        var room =
            await roomsService.GetFormByIdAsync(id);

        return room is null
            ? NotFound()
            : Ok(room);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] RoomFormDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (!await roomsService.BuildingExistsAsync(
                dto.BuildingId))
        {
            return BadRequest(new
            {
                message = "Wybrany budynek nie istnieje."
            });
        }

        if (await roomsService.RoomNumberExistsAsync(
                dto.RoomNumber,
                dto.BuildingId))
        {
            return Conflict(new
            {
                message =
                    "Sala o takim numerze już istnieje w wybranym budynku."
            });
        }

        await roomsService.CreateAsync(dto);

        return StatusCode(
            StatusCodes.Status201Created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] RoomFormDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest(new
            {
                message =
                    "Identyfikator w adresie nie zgadza się z identyfikatorem sali."
            });
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (!await roomsService.BuildingExistsAsync(
                dto.BuildingId))
        {
            return BadRequest(new
            {
                message = "Wybrany budynek nie istnieje."
            });
        }

        if (await roomsService.RoomNumberExistsAsync(
                dto.RoomNumber,
                dto.BuildingId,
                dto.Id))
        {
            return Conflict(new
            {
                message =
                    "Sala o takim numerze już istnieje w wybranym budynku."
            });
        }

        var updated =
            await roomsService.UpdateAsync(dto);

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
                await roomsService.DeleteAsync(id);

            return deleted
                ? NoContent()
                : NotFound();
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message =
                    "Nie można usunąć sali, ponieważ jest używana w planie zajęć."
            });
        }
    }
}