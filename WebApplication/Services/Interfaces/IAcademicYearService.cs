using WebApplication.DTOs.AcademicYear;

namespace WebApplication.Services.Interfaces;

public interface IAcademicYearService
{
    Task<IEnumerable<AcademicYearDto>> GetAllAsync();
    Task<AcademicYearDto?> GetByIdAsync(int? id);
    Task CreateAsync(AcademicYearFormDto dto);
    Task<bool> UpdateAsync(AcademicYearFormDto dto);
    Task DeleteAsync(int id);
}