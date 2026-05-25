using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.DTOs;
using WebApplication.DTOs.AcademicYear;
using WebApplication.Models;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services.ModelServices;

public class AcademicYearService(AppDbContext context) : IAcademicYearService
{
    public async Task<IEnumerable<AcademicYearIndexDto>> GetAllForIndexAsync()
    {
        return await context.AcademicYears
            .Select(ay => new AcademicYearIndexDto(
                ay.Id,
                ay.Name
            ))
            .ToListAsync();
    }

    public async Task<AcademicYearDetailsDto?> GetDetailsByIdAsync(int id)
    {
        return await context.AcademicYears
            .Where(ay => ay.Id == id)
            .Select(ay => new AcademicYearDetailsDto(
                ay.Id,
                ay.Name,
                new DateRangeDto
                {
                    StartDate = ay.StartDate,
                    EndDate = ay.EndDate
                }
            ))
            .FirstOrDefaultAsync();
    }

    public async Task<AcademicYearFormDto?> GetFormByIdAsync(int id)
    {
        return await context.AcademicYears
            .Where(ay => ay.Id == id)
            .Select(ay => new AcademicYearFormDto
            {
                Id = ay.Id,
                Period = new DateRangeDto
                {
                    StartDate = ay.StartDate,
                    EndDate = ay.EndDate
                }
            })
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(AcademicYearFormDto dto)
    {
        var generatedName = $"{dto.Period.StartDate.Year}/{dto.Period.EndDate.Year}";

        var academicYear = new AcademicYear
        {
            Name = generatedName,
            StartDate = dto.Period.StartDate,
            EndDate = dto.Period.EndDate
        };

        context.Add(academicYear);
        await context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(AcademicYearFormDto dto)
    {
        var academicYear = await context.AcademicYears.FindAsync(dto.Id);
        if (academicYear == null)
        {
            return false;
        }

        var generatedName = $"{dto.Period.StartDate.Year}/{dto.Period.EndDate.Year}";

        academicYear.Name = generatedName;
        academicYear.StartDate = dto.Period.StartDate;
        academicYear.EndDate = dto.Period.EndDate;

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
        return await context.AcademicYears.AnyAsync(ay => ay.Id == id);
    }
}