using WebApplication.DTOs.Student;

namespace WebApplication.Services.Interfaces;

public interface IStudentsService
{
    Task<IEnumerable<StudentDto>> GetAllAsync();
    Task<StudentDto?> GetByIdAsync(int id);
    Task<StudentDetailsDto?> GetDetailsByIdAsync(int id);
    Task<StudentFormDto?> GetFormByIdAsync(int id);
    Task CreateAsync(StudentFormDto dto);
    Task<bool> UpdateAsync(StudentFormDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);

    Task<List<StudentAdminItemDto>> GetAllForAdminAsync(
        StudentAdminFilterDto filter);

    Task<StudentAdminDetailsDto?> GetAdminDetailsAsync(int id);

    Task<StudentAdminMetadataDto> GetAdminMetadataAsync();

    Task<bool> CreateForAdminAsync(StudentAdminFormDto dto);

    Task<bool> UpdateForAdminAsync(StudentAdminFormDto dto);

    Task<bool> StudentNumberExistsAsync(
        string studentId,
        int? excludedStudentId = null);

    Task<bool> GroupsExistAsync(IEnumerable<int> groupIds);
}