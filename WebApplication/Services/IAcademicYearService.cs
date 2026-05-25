using WebApplication.Models;

namespace WebApplication.Services
{
    public interface IAcademicYearService
    {
        Task<IEnumerable<AcademicYear>> GetAllAsync();
        Task<AcademicYear?> GetByIdAsync(int id);
        Task CreateAsync(AcademicYear academicYear);
        Task UpdateAsync(AcademicYear academicYear);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}