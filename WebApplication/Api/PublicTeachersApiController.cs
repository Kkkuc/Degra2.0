using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication.DTOs.Teacher;
using WebApplication.Services.Interfaces;

namespace WebApplication.Api;

[ApiController]
[AllowAnonymous]
[Route("api/public/teachers")]
public class PublicTeachersApiController(
    ITeachersService teachersService)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TeacherPublicDto>>>
        GetAll()
    {
        return Ok(
            await teachersService.GetPublicListAsync());
    }
}