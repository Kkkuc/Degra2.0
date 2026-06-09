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
            .Select(s => new SubjectIndexDto(s.Id, s.Name))
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
    
    public async Task<List<SubjectAdminItemDto>> GetAllForAdminAsync(
        SubjectAdminFilterDto filter)
    {
        var query = context.Subjects
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim().ToLower();

            query = query.Where(subject =>
                subject.Name.ToLower().Contains(search) ||
                (subject.Abbreviation != null &&
                 subject.Abbreviation.ToLower().Contains(search)) ||
                (subject.Code != null &&
                 subject.Code.ToLower().Contains(search)));
        }

        return await query
            .OrderBy(subject => subject.Name)
            .Select(subject => new SubjectAdminItemDto(
                subject.Id,
                subject.Name,
                subject.Abbreviation,
                subject.Code))
            .ToListAsync();
    }
    
    public async Task<SubjectAdminMetadataDto>
        GetAdminMetadataAsync()
    {
        var suggestions = await context.Subjects
            .AsNoTracking()
            .Select(subject => subject.Name)
            .Distinct()
            .OrderBy(name => name)
            .ToListAsync();

        return new SubjectAdminMetadataDto
        {
            Suggestions = suggestions
        };
    }
    
    public async Task<bool> NameOrCodeExistsAsync(
        string name,
        string? code,
        int? excludedId = null)
    {
        var normalizedName = name.Trim().ToLower();
        var normalizedCode = string.IsNullOrWhiteSpace(code)
            ? null
            : code.Trim().ToLower();

        return await context.Subjects.AnyAsync(subject =>
            (!excludedId.HasValue ||
             subject.Id != excludedId.Value) &&
            (
                subject.Name.ToLower() == normalizedName ||
                (
                    normalizedCode != null &&
                    subject.Code != null &&
                    subject.Code.ToLower() == normalizedCode
                )
            ));
    }
}