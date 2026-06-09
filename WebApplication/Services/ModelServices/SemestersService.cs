using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.DTOs.Semester;
using WebApplication.Models;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services.ModelServices;

public class SemestersService(AppDbContext context) : ISemestersService
{
    public async Task<IEnumerable<SemesterIndexDto>> GetAllForIndexAsync()
    {
        return await context.Semesters
            .Select(s => new SemesterIndexDto(
                s.Id,
                s.Name,
                s.AcademicYear != null ? s.AcademicYear.Name : "Brak"
            ))
            .ToListAsync();
    }

    public async Task<SemesterDetailsDto?> GetDetailsByIdAsync(int id)
    {
        return await context.Semesters
            .Where(s => s.Id == id)
            .Select(s => new SemesterDetailsDto(
                s.Id,
                s.Name,
                s.AcademicYear != null ? s.AcademicYear.Name : "Brak",
                s.StartDate,
                s.EndDate
            ))
            .FirstOrDefaultAsync();
    }

    public async Task<SemesterFormDto?> GetFormByIdAsync(int id)
    {
        return await context.Semesters
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new SemesterFormDto
            {
                Id = s.Id,
                AcademicYearId = s.AcademicYearId,
                Name = s.Name,
                StartDate = s.StartDate,
                EndDate = s.EndDate
            })
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(SemesterFormDto dto)
    {
        var semester = new Semester
        {
            AcademicYearId = dto.AcademicYearId,
            Name = dto.Name.Trim(),
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
        };
        
        context.Semesters.Add(semester);
        await context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(SemesterFormDto dto)
    {
        var semester = await context.Semesters.FirstOrDefaultAsync(s => s.Id == dto.Id);
        if (semester == null)
        {
            return false;
        }

        semester.AcademicYearId = dto.AcademicYearId;
        semester.Name = dto.Name.Trim();
        semester.StartDate = dto.StartDate;
        semester.EndDate = dto.EndDate;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var semester = await context.Semesters.FirstOrDefaultAsync(s => s.Id == id);
        if (semester == null)
        {
            return false;
        }

        context.Semesters.Remove(semester);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Semesters.AnyAsync(e => e.Id == id);
    }

    public async Task<Dictionary<int, string>> GetAcademicYearsDropdownAsync()
    {
        return await context.AcademicYears
            .AsNoTracking()
            .OrderByDescending(year => year.StartDate)
            .ToDictionaryAsync(
                year => year.Id,
                year => year.Name);
    }
    
    public async Task<List<SemesterAdminItemDto>> GetAllForAdminAsync(
        SemesterFilterDto filter)
    {
        var query = context.Semesters
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            var name = filter.Name.Trim().ToLower();

            query = query.Where(semester =>
                semester.Name.ToLower().Contains(name));
        }

        if (filter.AcademicYearId.HasValue)
        {
            query = query.Where(semester =>
                semester.AcademicYearId ==
                filter.AcademicYearId.Value);
        }

        return await query
            .OrderByDescending(semester => semester.StartDate)
            .ThenBy(semester => semester.Name)
            .Select(semester => new SemesterAdminItemDto(
                semester.Id,
                semester.Name,
                semester.AcademicYearId,
                semester.AcademicYear != null
                    ? semester.AcademicYear.Name
                    : "Brak roku akademickiego",
                semester.StartDate,
                semester.EndDate))
            .ToListAsync();
    }
    
    public async Task<SemesterAdminMetadataDto>
        GetAdminMetadataAsync()
    {
        var names = await context.Semesters
            .AsNoTracking()
            .Select(semester => semester.Name)
            .Distinct()
            .OrderBy(name => name)
            .ToListAsync();

        var academicYears = await context.AcademicYears
            .AsNoTracking()
            .OrderByDescending(year => year.StartDate)
            .Select(year => new SemesterDropdownItemDto(
                year.Id,
                year.Name))
            .ToListAsync();

        return new SemesterAdminMetadataDto
        {
            NameSuggestions = names,
            AcademicYears = academicYears
        };
    }
    
    public async Task<bool> AcademicYearExistsAsync(
        int academicYearId)
    {
        return await context.AcademicYears
            .AnyAsync(year => year.Id == academicYearId);
    }
}