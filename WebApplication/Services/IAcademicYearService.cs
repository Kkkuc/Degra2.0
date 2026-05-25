using WebApplication.DTOs;
using WebApplication.Models;

namespace WebApplication.Services
{
    public interface IAcademicYearService
    {
        Task<IEnumerable<AcademicYearDto>> GetAllAsync();
        Task<AcademicYearDto?> GetByIdAsync(int id);
        Task CreateAsync(AcademicYear academicYear);
        Task UpdateAsync(AcademicYear academicYear);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}