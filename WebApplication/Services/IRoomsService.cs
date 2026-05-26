using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Services
{
    public interface IRoomsService
    {
        Task<IEnumerable<Room>> GetAllWithBuildingAsync();
        Task<Room?> GetByIdWithBuildingAsync(int id);
        Task<Room?> GetByIdAsync(int id);
        Task CreateAsync(Room room);
        Task UpdateAsync(Room room);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        
        // Metoda do pobrania budynków pod SelectList w formularzach
        Task<IEnumerable<Building>> GetAllBuildingsAsync();
    }
}