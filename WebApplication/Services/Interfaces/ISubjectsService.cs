using WebApplication.Models;

namespace WebApplication.Services;

public interface ISubjectsService
{
    Task<IEnumerable<Subject>> GetAllAsync();
    Task<Subject?> GetByIdAsync(int id);
    Task CreateAsync(Subject subject);
    Task UpdateAsync(Subject subject);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}