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
                .Select(ay => new AcademicYearDto(
                    ay.Id,
                    ay.Name,
                    ay.StartDate,
                    ay.EndDate
                ))
                .ToListAsync();
        }

        public async Task<AcademicYearDto?> GetByIdAsync(int? id)
        {
            return await context.AcademicYears
                .Where(ay => ay.Id == id)
                .Select(ay => new AcademicYearDto(
                    ay.Id,
                    ay.Name,
                    ay.StartDate,
                    ay.EndDate
                )) 
                .FirstOrDefaultAsync();
        }
        
        public async Task<AcademicYearFormDto?> GetFormByIdAsync(int? id)
        {
            return await context.AcademicYears
                .Where(ay => ay.Id == id)
                .Select(ay => new AcademicYearFormDto
                {
                    Id = ay.Id,
                    Name = ay.Name,
                    StartDate = ay.StartDate,
                    EndDate = ay.EndDate
                })
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(AcademicYearFormDto dto)
        {
            var academicYear = new AcademicYear
            {
                Name = dto.Name,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            context.Add(academicYear);
            await context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(AcademicYearFormDto dto)
        {
            var academicYear = await context.AcademicYears.FindAsync(dto.Id);
            if (academicYear == null) return false;

            // Przepisujemy wartości z DTO do encji śledzonej przez EF Core
            academicYear.Name = dto.Name;
            academicYear.StartDate = dto.StartDate;
            academicYear.EndDate = dto.EndDate;

            context.Update(academicYear);
            await context.SaveChangesAsync();
            return true;
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