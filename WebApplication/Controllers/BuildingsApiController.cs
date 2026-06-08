using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication.DTOs.Building;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

[ApiController]
[Authorize(Roles = "Moderator")]
[Route("api/buildings")]
public class BuildingsApiController(IBuildingsService buildingsService) : ControllerBase
{
    [HttpGet("metadata")]
    public async Task<IActionResult> GetMetadata()
    {
        return Ok(await buildingsService.GetAdminMetadataAsync());
    }

    [HttpGet]
    public async Task<IActionResult> GetBuildings([FromQuery] string? name, [FromQuery] string? address, [FromQuery] int? addressId, [FromQuery] int? facultyId)
    {
        if (addressId.HasValue)
        {
            var selectedAddress = await buildingsService.GetFormByIdAsync(addressId.Value);
            address = selectedAddress == null
                ? address
                : $"{selectedAddress.AddressDto.Street} {selectedAddress.AddressDto.HouseNumber}, {selectedAddress.AddressDto.PostalCode} {selectedAddress.AddressDto.City}";
        }

        var buildings = await buildingsService.GetAllForAdminAsync(name, address, facultyId);
        return Ok(buildings);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetBuilding(int id)
    {
        var building = await buildingsService.GetFormByIdAsync(id);
        return building == null ? NotFound() : Ok(building);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBuilding([FromBody] BuildingFormDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        await buildingsService.CreateAsync(dto);
        return Ok();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateBuilding(int id, [FromBody] BuildingFormDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var success = await buildingsService.UpdateAsync(dto);
        return success ? Ok() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBuilding(int id)
    {
        await buildingsService.DeleteAsync(id);
        return NoContent();
    }
}
