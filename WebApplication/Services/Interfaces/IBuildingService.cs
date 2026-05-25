using WebApplication.DTOs.Building;

namespace WebApplication.Services.Interfaces;

public interface IBuildingsService
{
    Task<IEnumerable<BuildingIndexDto>> GetAllForIndexAsync(); 
    Task<BuildingDetailsDto?> GetDetailsByIdAsync(int id);      
    Task<BuildingFormDto?> GetFormByIdAsync(int id);
    Task CreateAsync(BuildingFormDto dto);
    Task<bool> UpdateAsync(BuildingFormDto dto);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<IEnumerable<KeyValuePair<int, string>>> GetFacultyDropdownListAsync(); 
}