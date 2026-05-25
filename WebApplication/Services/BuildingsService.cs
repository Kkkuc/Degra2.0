using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class BuildingsService(AppDbContext context) : IBuildingsService
    {
        public async Task<IEnumerable<Building>> GetAllWithFacultyAsync()
        {
            return await context.Buildings
                .Include(b => b.Faculty)
                .ToListAsync();
        }

        public async Task<Building?> GetByIdWithFacultyAsync(int id)
        {
            return await context.Buildings
                .Include(b => b.Faculty)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Building?> GetByIdAsync(int id)
        {
            return await context.Buildings.FindAsync(id);
        }

        public async Task CreateAsync(Building building)
        {
            context.Add(building);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Building building)
        {
            context.Update(building);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var building = await context.Buildings.FindAsync(id);
            if (building != null)
            {
                context.Buildings.Remove(building);
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await context.Buildings.AnyAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Faculty>> GetAllFacultiesAsync()
        {
            return await context.Faculties.ToListAsync();
        }
    }
}