using WebApplication.DTOs.Teacher;

namespace WebApplication.Services.Interfaces;

public interface ITeachersService
{
    Task<IEnumerable<TeacherDto>> GetAllAsync();
    Task<TeacherDto?> GetByIdAsync(int id);
    Task<TeacherFormDto?> GetFormByIdAsync(int id);
    Task CreateAsync(TeacherFormDto dto);
    Task<bool> UpdateAsync(TeacherFormDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    
    Task<List<TeacherPublicDto>> GetPublicListAsync();

    Task<List<TeacherAdminItemDto>> GetAllForAdminAsync(
        TeacherAdminFilterDto filter);

    Task<TeacherAdminMetadataDto> GetAdminMetadataAsync();

    Task<bool> EmailExistsAsync(
        string email,
        int? excludedTeacherId = null);
}