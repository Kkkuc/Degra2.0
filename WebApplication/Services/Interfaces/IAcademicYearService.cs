using WebApplication.DTOs.AcademicYear;

namespace WebApplication.Services.Interfaces
{
    public interface IAcademicYearService
    {
        Task<IEnumerable<AcademicYearIndexDto>> GetAllForIndexAsync();
        Task<AcademicYearDetailsDto?> GetDetailsByIdAsync(int id);
        Task<AcademicYearFormDto?> GetFormByIdAsync(int id);
        Task CreateAsync(AcademicYearFormDto dto);
        Task<bool> UpdateAsync(AcademicYearFormDto dto);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}