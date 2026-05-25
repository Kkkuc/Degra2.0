using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class AcademicYearService(AppDbContext context) : IAcademicYearService
    {
        public async Task<IEnumerable<AcademicYear>> GetAllAsync()
        {
            return await context.AcademicYears.ToListAsync();
        }

        public async Task<AcademicYear?> GetByIdAsync(int id)
        {
            return await context.AcademicYears.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task CreateAsync(AcademicYear academicYear)
        {
            context.Add(academicYear);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AcademicYear academicYear)
        {
            context.Update(academicYear);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var academicYear = await context.AcademicYears.FindAsync(id);
            if (academicYear != null)
            {
                context.AcademicYears.Remove(academicYear);
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await context.AcademicYears.AnyAsync(e => e.Id == id);
        }
    }
}