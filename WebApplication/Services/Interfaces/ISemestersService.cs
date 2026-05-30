using WebApplication.DTOs.Semester;
using WebApplication.Models;

namespace WebApplication.Services.Interfaces;

public interface ISemestersService
{
    Task<IEnumerable<SemesterIndexDto>> GetAllForIndexAsync();
    Task<SemesterDetailsDto?> GetDetailsByIdAsync(int id);
    Task<SemesterFormDto?> GetFormByIdAsync(int id);
    Task CreateAsync(SemesterFormDto dto);
    Task<bool> UpdateAsync(SemesterFormDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    
    // Słownik pod SelectList na widoku (Id -> Nazwa roku)
    Task<Dictionary<int, string>> GetAcademicYearsDropdownAsync();
}