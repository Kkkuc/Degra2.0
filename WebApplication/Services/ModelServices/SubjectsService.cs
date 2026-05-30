using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.DTOs.Subject;
using WebApplication.Models;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services.ModelServices;

public class SubjectsService(AppDbContext context) : ISubjectsService
{
    public async Task<IEnumerable<SubjectIndexDto>> GetAllForIndexAsync()
    {
        var rand = new Random();
        return await context.Subjects
            .Select(s => new SubjectIndexDto(s.Id, s.Name, $"#{rand.Next(0x1000000)}"))
            .ToListAsync();
    }

    public async Task<SubjectDetailsDto?> GetDetailsByIdAsync(int id)
    {
        return await context.Subjects
            .Where(s => s.Id == id)
            .Select(s => new SubjectDetailsDto(s.Id, s.Name, s.Abbreviation, s.Code))
            .FirstOrDefaultAsync();
    }

    public async Task<SubjectFormDto?> GetFormByIdAsync(int id)
    {
        return await context.Subjects
            .Where(s => s.Id == id)
            .Select(s => new SubjectFormDto
            {
                Id = s.Id,
                Name = s.Name,
                Abbreviation = s.Abbreviation,
                Code = s.Code
            })
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(SubjectFormDto dto)
    {
        var subject = new Subject
        {
            Name = dto.Name,
            Abbreviation = dto.Abbreviation,
            Code = dto.Code
        };

        context.Subjects.Add(subject);
        await context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(SubjectFormDto dto)
    {
        var subject = await context.Subjects.FirstOrDefaultAsync(s => s.Id == dto.Id);
        if (subject == null) return false;

        subject.Name = dto.Name;
        subject.Abbreviation = dto.Abbreviation;
        subject.Code = dto.Code;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var subject = await context.Subjects.FirstOrDefaultAsync(s => s.Id == id);
        if (subject == null) return false;

        context.Subjects.Remove(subject);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Subjects.AnyAsync(e => e.Id == id);
    }
}