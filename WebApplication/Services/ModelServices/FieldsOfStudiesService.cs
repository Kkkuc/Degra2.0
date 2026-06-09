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
        return await context.FieldsOfStudy
            .Select(f => new FieldOfStudyIndexDto(
                f.Id,
                f.Name,
                f.Degree,               // Przekazujemy stopień
                (int)f.Mode,            // Rzutujemy enum na int
                f.Faculty!.Abbreviation // Przekazujemy skrót wydziału
            ))
            .ToListAsync();
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
    public async Task<IEnumerable<string>> GetUniqueNamesAsync()
    {
        return await context.FieldsOfStudy
            .Select(f => f.Name)
            .Distinct()
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
    
    public async Task<IEnumerable<FieldOfStudyIndexDto>> GetFilteredAsync(FieldOfStudyFilterDto filter)
    {
        var query = context.FieldsOfStudy.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            query = query.Where(f => f.Name.Contains(filter.Name));
        }

        if (filter.FacultyId.HasValue)
        {
            query = query.Where(f => f.FacultyId == filter.FacultyId);
        }

        if (filter.Mode.HasValue)
        {
            query = query.Where(f => (int)f.Mode == filter.Mode);
        }
        
        if (!string.IsNullOrWhiteSpace(filter.Degree))
        {
            query = query.Where(f => f.Degree.Contains(filter.Degree));
        }

        // Pobieramy pełny zestaw danych, który pasuje do Twojego DTO
        return await query
            .Select(f => new FieldOfStudyIndexDto(
                f.Id,
                f.Name,
                f.Degree,               // Dodano
                (int)f.Mode,            // Dodano (rzutowanie na int)
                f.Faculty!.Abbreviation // Dodano (zakładając, że to pole jest w DTO)
            ))
            .ToListAsync();
    }
    
    public async Task<IEnumerable<FieldOfStudyIndexDto>> GetPaginatedAsync(int page, int pageSize)
    {
        return await context.FieldsOfStudy
            .OrderBy(f => f.Name) // Ważne: stronicowanie wymaga sortowania!
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(f => new FieldOfStudyIndexDto(f.Id, f.Name, f.Degree, (int)f.Mode, f.Faculty!.Abbreviation))
            .ToListAsync();
    }
}