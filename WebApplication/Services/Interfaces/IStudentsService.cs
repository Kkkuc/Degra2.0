using WebApplication.Models;

namespace WebApplication.Services;

public interface IStudentsService
{
    Task<IEnumerable<Student>> GetAllAsync();
    Task<Student?> GetByIdAsync(int id);
    Task CreateAsync(Student student);
    Task UpdateAsync(Student student);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}