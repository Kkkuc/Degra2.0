using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.DTOs.Student;
using WebApplication.Models;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services.ModelServices;

public class StudentsService(AppDbContext context) : IStudentsService
{
    public async Task<IEnumerable<StudentDto>> GetAllAsync()
    {
        return await context.Students
            .Select(s => new StudentDto(s.Id, s.FirstName + " " + s.LastName))
            .ToListAsync();
    }

    public async Task<StudentDto?> GetByIdAsync(int id)
    {
        return await context.Students
            .Where(s => s.Id == id)
            .Select(s => new StudentDto(s.Id, s.FirstName + " " + s.LastName))
            .FirstOrDefaultAsync();
    }

    public async Task<StudentDetailsDto?> GetDetailsByIdAsync(int id)
    {
        return await context.Students
            .Where(s => s.Id == id)
            .Select(s => new StudentDetailsDto(s.Id, s.FirstName, s.LastName, s.StudentID))
            .FirstOrDefaultAsync();
    }

    public async Task<StudentFormDto?> GetFormByIdAsync(int id)
    {
        return await context.Students
            .Where(s => s.Id == id)
            .Select(s => new StudentFormDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                StudentId = s.StudentID
            })
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(StudentFormDto dto)
    {
        var student = new Student
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            StudentID = dto.StudentId
        };

        context.Students.Add(student);
        await context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(StudentFormDto dto)
    {
        var student = await context.Students.FirstOrDefaultAsync(s => s.Id == dto.Id);
        if (student == null) return false;

        student.FirstName = dto.FirstName;
        student.LastName = dto.LastName;
        student.StudentID = dto.StudentId;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var student = await context.Students.FirstOrDefaultAsync(s => s.Id == id);
        if (student == null) return false;

        context.Students.Remove(student);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Students.AnyAsync(e => e.Id == id);
    }
}