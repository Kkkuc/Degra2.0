using WebApplication.DTOs.Group;

namespace WebApplication.Services.Interfaces;

public interface IGroupsService
{
    Task<IEnumerable<GroupIndexDto>> GetAllForIndexAsync();
    Task<GroupDetailsDto?> GetDetailsByIdAsync(int id);
    Task<GroupFormDto?> GetFormByIdAsync(int id);
    Task CreateAsync(GroupFormDto dto);
    Task<bool> UpdateAsync(GroupFormDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    
    Task<Dictionary<int, string>> GetFieldsOfStudyDropdownListAsync();
    Task<Dictionary<int, string>> GetSemestersDropdownListAsync();
    Task<Dictionary<int, string>> GetSpecializationsDropdownListAsync();
    
    Task<List<GroupAdminItemDto>> GetAllForAdminAsync(
        GroupFilterDto filter);

    Task<GroupAdminMetadataDto> GetAdminMetadataAsync();

    Task<bool> IsValidForeignKeysAsync(GroupFormDto dto);
}