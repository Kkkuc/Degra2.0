using WebApplication.Models;

namespace WebApplication.Services;

public interface ITeachersService
{
    Task<IEnumerable<Teacher>> GetAllAsync();
    Task<Teacher?> GetByIdAsync(int id);
    Task CreateAsync(Teacher teacher);
    Task UpdateAsync(Teacher teacher);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}