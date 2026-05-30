using WebApplication.DTOs.StudentGroup;

namespace WebApplication.Services.Interfaces;

public interface IStudentGroupsService
{
    Task<IEnumerable<StudentGroupDto>> GetAllWithRelationsAsync();
    Task<StudentGroupDto?> GetByStudentIdWithRelationsAsync(int studentId);
    Task<StudentGroupFormDto?> GetFormByStudentIdAsync(int studentId);
    Task CreateAsync(StudentGroupFormDto dto);
    Task<bool> UpdateAsync(int originalStudentId, StudentGroupFormDto dto);
    Task<bool> DeleteAsync(int studentId);
    Task<bool> ExistsAsync(int studentId);
    
    Task<Dictionary<int, string>> GetGroupsDropdownAsync();
    Task<IEnumerable<StudentLookupDto>> GetStudentsLookupAsync();
}