using WebApplication.DTOs.Specialization;
using WebApplication.Models;

namespace WebApplication.Services.Interfaces;

public interface ISpecializationsService
{
    Task<IEnumerable<SpecializationDto>> GetAllAsync();
    Task<SpecializationDto?> GetByIdAsync(int id);
    Task CreateAsync(SpecializationDto dto);
    Task<bool> UpdateAsync(SpecializationDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}