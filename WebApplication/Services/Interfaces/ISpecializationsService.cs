using WebApplication.DTOs.Specialization;
using WebApplication.Models;

namespace WebApplication.Services.Interfaces;

public interface ISpecializationsService
{
    Task<List<SpecializationDto>> GetAllAsync(SpecializationFilterDto? filter = null);

    Task<SpecializationDto?> GetByIdAsync(int id);

    Task<SpecializationMetadataDto> GetMetadataAsync();

    Task CreateAsync(SpecializationDto dto);

    Task<bool> UpdateAsync(SpecializationDto dto);

    Task<bool> DeleteAsync(int id);

    Task<bool> ExistsAsync(int id);

    Task<bool> NameExistsAsync(string name, int? excludedId = null);
}