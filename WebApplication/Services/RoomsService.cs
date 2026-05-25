using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class RoomsService(AppDbContext context) : IRoomsService
    {
        public async Task<IEnumerable<Room>> GetAllWithBuildingAsync()
        {
            return await context.Rooms
                .Include(r => r.Building)
                .ToListAsync();
        }

        public async Task<Room?> GetByIdWithBuildingAsync(int id)
        {
            return await context.Rooms
                .Include(r => r.Building)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Room?> GetByIdAsync(int id)
        {
            return await context.Rooms.FindAsync(id);
        }

        public async Task CreateAsync(Room room)
        {
            context.Add(room);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Room room)
        {
            context.Update(room);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var room = await context.Rooms.FindAsync(id);
            if (room != null)
            {
                context.Rooms.Remove(room);
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await context.Rooms.AnyAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Building>> GetAllBuildingsAsync()
        {
            return await context.Buildings.ToListAsync();
        }
    }
}