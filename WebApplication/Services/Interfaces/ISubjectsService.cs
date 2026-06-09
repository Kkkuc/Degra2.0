using WebApplication.DTOs.Subject;
using WebApplication.Models;
using SubjectIndexDto = WebApplication.DTOs.Subject.SubjectIndexDto;

namespace WebApplication.Services.Interfaces;

public interface ISubjectsService
{
    Task<IEnumerable<SubjectIndexDto>> GetAllForIndexAsync();
    Task<SubjectDetailsDto?> GetDetailsByIdAsync(int id);
    Task<SubjectFormDto?> GetFormByIdAsync(int id);
    Task CreateAsync(SubjectFormDto dto);
    Task<bool> UpdateAsync(SubjectFormDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    
    Task<List<SubjectAdminItemDto>> GetAllForAdminAsync(
        SubjectAdminFilterDto filter);

    Task<SubjectAdminMetadataDto> GetAdminMetadataAsync();

    Task<bool> NameOrCodeExistsAsync(
        string name,
        string? code,
        int? excludedId = null);
}