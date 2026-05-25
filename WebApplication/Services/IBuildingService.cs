using WebApplication.Models;

namespace WebApplication.Services
{
    public interface IBuildingsService
    {
        Task<IEnumerable<Building>> GetAllWithFacultyAsync();
        Task<Building?> GetByIdWithFacultyAsync(int id);
        Task<Building?> GetByIdAsync(int id);
        Task CreateAsync(Building building);
        Task UpdateAsync(Building building);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        
        // Potrzebne do wypełnienia SelectList w widokach formularzy
        Task<IEnumerable<Faculty>> GetAllFacultiesAsync();
    }
}



namespace WebApplication.Services
{
}