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
            Name = dto.Name,
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
        semester.Name = dto.Name;
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
            .ToDictionaryAsync(a => a.Id, a => a.Name);
    }
}