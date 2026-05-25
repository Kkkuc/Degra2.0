using WebApplication.DTOs.Faculty;
using WebApplication.Models;

namespace WebApplication.Services.Interfaces;

public interface IFacultiesService
{
    Task<IEnumerable<FacultyDto>> GetAllAsync();
    Task<FacultyDto?> GetByIdAsync(int id);
    Task CreateAsync(FacultyDto dto);
    Task<bool> UpdateAsync(FacultyDto dto);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}