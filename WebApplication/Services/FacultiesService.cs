using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class FacultiesService(AppDbContext context) : IFacultiesService
    {
        public async Task<IEnumerable<Faculty>> GetAllAsync()
        {
            return await context.Faculties.ToListAsync();
        }

        public async Task<Faculty?> GetByIdAsync(int id)
        {
            return await context.Faculties.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task CreateAsync(Faculty faculty)
        {
            context.Add(faculty);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Faculty faculty)
        {
            context.Update(faculty);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var faculty = await context.Faculties.FindAsync(id);
            if (faculty != null)
            {
                context.Faculties.Remove(faculty);
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await context.Faculties.AnyAsync(e => e.Id == id);
        }
    }
}