using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.DTOs.FieldOfStudy;
using WebApplication.Models;
using WebApplication.Models.enums;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services.ModelServices;

public class FieldsOfStudiesService(AppDbContext context) : IFieldsOfStudiesService
{
    public async Task<IEnumerable<FieldOfStudyIndexDto>> GetAllForIndexAsync()
    {
        var list = await context.FieldsOfStudy
            .Select(f => new
            {
                f.Id,
                f.Name,
                f.Degree,
                FacultyAbbreviation = f.Faculty!.Abbreviation,
                f.Mode
            })
            .ToListAsync();

        return list.Select(f => new FieldOfStudyIndexDto(
            f.Id,
            f.Name
        ));
    }

    public async Task<FieldOfStudyDetailsDto?> GetDetailsByIdAsync(int id)
    {
        var f = await context.FieldsOfStudy
            .Where(f => f.Id == id)
            .Select(f => new
            {
                f.Id,
                f.Name,
                f.Degree,
                FacultyName = f.Faculty!.Name,
                FacultyAbbreviation = f.Faculty!.Abbreviation,
                f.Mode
            })
            .FirstOrDefaultAsync();

        if (f == null)
        {
            return null;
        }

        return new FieldOfStudyDetailsDto(
            f.Id,
            f.Name,
            f.Degree,
            f.FacultyName,
            GetEnumDisplayName(f.Mode)
        );
    }

    public async Task<FieldOfStudyFormDto?> GetFormByIdAsync(int id)
    {
        return await context.FieldsOfStudy
            .Where(f => f.Id == id)
            .Select(f => new FieldOfStudyFormDto
            {
                Id = f.Id,
                Name = f.Name,
                Degree = f.Degree,
                FacultyId = f.FacultyId,
                Mode = f.Mode
            })
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(FieldOfStudyFormDto dto)
    {
        var fieldOfStudy = new FieldOfStudy
        {
            Name = dto.Name,
            Degree = dto.Degree,
            FacultyId = dto.FacultyId,
            Mode = dto.Mode
        };

        context.Add(fieldOfStudy);
        await context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(FieldOfStudyFormDto dto)
    {
        var fieldOfStudy = await context.FieldsOfStudy.FindAsync(dto.Id);
        if (fieldOfStudy == null)
        {
            return false;
        }

        fieldOfStudy.Name = dto.Name;
        fieldOfStudy.Degree = dto.Degree;
        fieldOfStudy.FacultyId = dto.FacultyId;
        fieldOfStudy.Mode = dto.Mode;

        context.Update(fieldOfStudy);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task DeleteAsync(int id)
    {
        var fieldOfStudy = await context.FieldsOfStudy.FindAsync(id);
        if (fieldOfStudy != null)
        {
            context.FieldsOfStudy.Remove(fieldOfStudy);
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.FieldsOfStudy.AnyAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<KeyValuePair<int, string>>> GetFacultyDropdownListAsync()
    {
        return await context.Faculties
            .Select(f => new KeyValuePair<int, string>(f.Id, f.Abbreviation))
            .ToListAsync();
    }

    private static string GetEnumDisplayName(StudyMode mode)
    {
        var displayAttribute = mode.GetType()
            .GetMember(mode.ToString())
            .FirstOrDefault()?
            .GetCustomAttribute<DisplayAttribute>();

        return displayAttribute?.Name ?? mode.ToString();
    }
}