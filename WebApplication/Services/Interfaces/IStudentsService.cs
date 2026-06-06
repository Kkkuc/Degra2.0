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
}