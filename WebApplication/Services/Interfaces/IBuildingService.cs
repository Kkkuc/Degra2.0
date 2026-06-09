using WebApplication.DTOs.Building;

namespace WebApplication.Services.Interfaces;

public interface IBuildingsService
{
    Task<IEnumerable<BuildingAdminItemDto>> GetAllForAdminAsync(
        string? name = null, 
        int? addressId = null, 
        int? facultyId = null);
    Task<BuildingAdminMetadataDto> GetAdminMetadataAsync();
    Task<BuildingFormDto?> GetFormByIdAsync(int id);
    Task CreateAsync(BuildingFormDto dto);
    Task<bool> UpdateAsync(BuildingFormDto dto);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
