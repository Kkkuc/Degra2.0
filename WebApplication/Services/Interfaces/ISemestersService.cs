using WebApplication.Models;

namespace WebApplication.Services.Interfaces;

public interface ISemestersService
{
    Task<IEnumerable<Semester>> GetAllWithAcademicYearAsync();
    Task<Semester?> GetByIdWithAcademicYearAsync(int id);
    Task<Semester?> GetByIdAsync(int id);
    Task CreateAsync(Semester semester);
    Task UpdateAsync(Semester semester);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
        
    // Metoda do pobrania lat akademickich pod SelectList
    Task<IEnumerable<AcademicYear>> GetAllAcademicYearsAsync();
}