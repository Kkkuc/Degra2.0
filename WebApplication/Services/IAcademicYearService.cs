using WebApplication.DTOs;
using WebApplication.Models;

namespace WebApplication.Services;

public interface IAcademicYearService
{
    Task<IEnumerable<AcademicYearDto>> GetAllAsync();
    Task<AcademicYearDto?> GetByIdAsync(int? id);
    Task CreateAsync(AcademicYearFormDto dto);
    Task<bool> UpdateAsync(AcademicYearFormDto dto);
    Task DeleteAsync(int id);
}