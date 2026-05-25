using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.DTOs.Faculty;
using WebApplication.Models;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services.ModelServices
{
    public class FacultiesService(AppDbContext context) : IFacultiesService
    {
        public async Task<IEnumerable<FacultyDto>> GetAllAsync()
        {
            return await context.Faculties
                .Select(b => new FacultyDto(
                    b.Id,
                    b.Name,
                    b.Abbreviation
                ))
                .ToListAsync();
        }

        public async Task<FacultyDto?> GetByIdAsync(int id)
        {
            return await context.Faculties
                .Where(f => f.Id == id)
                .Select(f => new FacultyDto(
                    f.Id,
                    f.Name,
                    f.Abbreviation
                ))
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(FacultyDto dto)
        {
            var faculty = new Faculty
            {
                Name = dto.Name,
                Abbreviation = dto.Abbreviation
            };

            context.Add(faculty);
            await context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(FacultyDto dto)
        {
            var faculty = await context.Faculties.FindAsync(dto.Id);
            if (faculty == null)
            {
                return false;
            }

            faculty.Name = dto.Name;
            faculty.Abbreviation = dto.Abbreviation;

            context.Update(faculty);
            await context.SaveChangesAsync();
            return true;
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