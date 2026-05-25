using WebApplication.Models;

namespace WebApplication.Services.Interfaces;

public interface IFacultiesService
{
    Task<IEnumerable<Faculty>> GetAllAsync();
    Task<Faculty?> GetByIdAsync(int id);
    Task CreateAsync(Faculty faculty);
    Task UpdateAsync(Faculty faculty);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}