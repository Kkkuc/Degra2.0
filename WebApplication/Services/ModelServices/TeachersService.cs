using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.DTOs.Teacher;
using WebApplication.Models;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services.ModelServices;

public class TeachersService(AppDbContext context) : ITeachersService
{
    public async Task<IEnumerable<TeacherDto>> GetAllAsync()
    {
        return await context.Teachers
            .Select(t => new TeacherDto(
                t.Id,
                t.FirstName + " " + t.LastName,
                !string.IsNullOrEmpty(t.AcademicTitle) ? t.AcademicTitle + " " : "",
                t.Email ?? "Brak"
            ))
            .ToListAsync();
    }

    public async Task<TeacherDto?> GetByIdAsync(int id)
    {
        return await context.Teachers
            .Where(t => t.Id == id)
            .Select(t => new TeacherDto(
                t.Id,
                t.FirstName + " " + t.LastName,
                !string.IsNullOrEmpty(t.AcademicTitle) ? t.AcademicTitle + " " : "",
                t.Email ?? "Brak"
            ))
            .FirstOrDefaultAsync();
    }

    public async Task<TeacherFormDto?> GetFormByIdAsync(int id)
    {
        return await context.Teachers
            .Where(t => t.Id == id)
            .Select(t => new TeacherFormDto
            {
                Id = t.Id,
                AcademicTitle = t.AcademicTitle,
                FirstName = t.FirstName,
                LastName = t.LastName,
                Email = t.Email ?? string.Empty
            })
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(TeacherFormDto dto)
    {
        var teacher = new Teacher
        {
            AcademicTitle = dto.AcademicTitle,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email
        };

        context.Teachers.Add(teacher);
        await context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(TeacherFormDto dto)
    {
        var teacher = await context.Teachers.FirstOrDefaultAsync(t => t.Id == dto.Id);
        if (teacher == null) return false;

        teacher.AcademicTitle = dto.AcademicTitle;
        teacher.FirstName = dto.FirstName;
        teacher.LastName = dto.LastName;
        teacher.Email = dto.Email;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var teacher = await context.Teachers.FirstOrDefaultAsync(t => t.Id == id);
        if (teacher == null) return false;

        context.Teachers.Remove(teacher);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Teachers.AnyAsync(e => e.Id == id);
    }
}