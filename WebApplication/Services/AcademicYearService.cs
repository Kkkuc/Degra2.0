using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.DTOs;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class AcademicYearService(AppDbContext context) : IAcademicYearService
    {
        public async Task<IEnumerable<AcademicYearDto>> GetAllAsync()
        {
            return await context.AcademicYears
                .Select(ay => new AcademicYearDto
                {
                    Id = ay.Id,
                    Name = ay.Name,
                    StartDate = ay.StartDate,
                    EndDate = ay.EndDate
                })
                .ToListAsync();
        }

        public async Task<AcademicYearDto?> GetByIdAsync(int id)
        {
            return await context.AcademicYears
                .Where(ay => ay.Id == id)
                .Select(ay => new AcademicYearDto
                {
                    Id = ay.Id,
                    Name = ay.Name,
                    StartDate = ay.StartDate,
                    EndDate = ay.EndDate
                })
                .FirstOrDefaultAsync();
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