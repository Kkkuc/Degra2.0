using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services
{
    public class FieldsOfStudiesService(AppDbContext context) : IFieldsOfStudiesService
    {
        public async Task<IEnumerable<FieldOfStudy>> GetAllWithFacultyAsync()
        {
            return await context.FieldsOfStudy
                .Include(f => f.Faculty)
                .ToListAsync();
        }

        public async Task<FieldOfStudy?> GetByIdWithFacultyAsync(int id)
        {
            return await context.FieldsOfStudy
                .Include(f => f.Faculty)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<FieldOfStudy?> GetByIdAsync(int id)
        {
            return await context.FieldsOfStudy.FindAsync(id);
        }

        public async Task CreateAsync(FieldOfStudy fieldOfStudy)
        {
            context.Add(fieldOfStudy);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FieldOfStudy fieldOfStudy)
        {
            context.Update(fieldOfStudy);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var fieldOfStudy = await context.FieldsOfStudy.FindAsync(id);
            if (fieldOfStudy != null)
            {
                context.FieldsOfStudy.Remove(fieldOfStudy);
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await context.FieldsOfStudy.AnyAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Faculty>> GetAllFacultiesAsync()
        {
            return await context.Faculties.ToListAsync();
        }
    }
}