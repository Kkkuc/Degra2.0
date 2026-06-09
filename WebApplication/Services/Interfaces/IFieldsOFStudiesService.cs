using WebApplication.DTOs.FieldOfStudy;

namespace WebApplication.Services.Interfaces;

public interface IFieldsOfStudiesService
{
    Task<IEnumerable<FieldOfStudyIndexDto>> GetAllForIndexAsync();

    Task<FieldOfStudyDetailsDto?> GetDetailsByIdAsync(int id);

    Task<FieldOfStudyFormDto?> GetFormByIdAsync(int id);

    Task CreateAsync(FieldOfStudyFormDto dto);

    Task<bool> UpdateAsync(FieldOfStudyFormDto dto);

    Task DeleteAsync(int id);

    Task<bool> ExistsAsync(int id);

    Task<IEnumerable<KeyValuePair<int, string>>> GetFacultyDropdownListAsync();
    Task<IEnumerable<FieldOfStudyIndexDto>> GetFilteredAsync(FieldOfStudyFilterDto filter);
    Task<IEnumerable<string>> GetUniqueNamesAsync();
}