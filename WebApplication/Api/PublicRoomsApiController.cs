using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication.DTOs.Room;
using WebApplication.Services.Interfaces;

namespace WebApplication.Api;

[ApiController]
[AllowAnonymous]
[Route("api/public/rooms")]
public class PublicRoomsApiController(
    IRoomsService roomsService)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<RoomPublicDto>>> GetAll()
    {
        return Ok(
            await roomsService.GetPublicListAsync());
    }
}