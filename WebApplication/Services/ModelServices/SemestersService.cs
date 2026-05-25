using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services
{
    public class SemestersService(AppDbContext context) : ISemestersService
    {
        public async Task<IEnumerable<Semester>> GetAllWithAcademicYearAsync()
        {
            return await context.Semesters
                .Include(s => s.AcademicYear)
                .ToListAsync();
        }

        public async Task<Semester?> GetByIdWithAcademicYearAsync(int id)
        {
            return await context.Semesters
                .Include(s => s.AcademicYear)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Semester?> GetByIdAsync(int id)
        {
            return await context.Semesters.FindAsync(id);
        }

        public async Task CreateAsync(Semester semester)
        {
            context.Add(semester);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Semester semester)
        {
            context.Update(semester);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var semester = await context.Semesters.FindAsync(id);
            if (semester != null)
            {
                context.Semesters.Remove(semester);
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await context.Semesters.AnyAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<AcademicYear>> GetAllAcademicYearsAsync()
        {
            return await context.AcademicYears.ToListAsync();
        }
    }
}