using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.DTOs.Group;
using WebApplication.Models;
using WebApplication.Models.enums;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services.ModelServices;

public class GroupsService(AppDbContext context) : IGroupsService
{
    public async Task<IEnumerable<GroupIndexDto>> GetAllForIndexAsync()
    {
        return await context.Groups
            .Select(g => new GroupIndexDto(
                g.Id,
                g.Name,
                g.Semester != null ? g.Semester.Name : "Brak"
            ))
            .ToListAsync();
    }

    public async Task<GroupDetailsDto?> GetDetailsByIdAsync(int id)
    {
        return await context.Groups
            .Where(g => g.Id == id)
            .Select(g => new GroupDetailsDto(
                g.Id,
                g.Name,
                g.Semester != null ? g.Semester.Name : "Brak",
                g.FieldOfStudy != null ? g.FieldOfStudy.Name : "Brak",
                g.Specialization != null ? g.Specialization.Name : "Brak",
                g.ClassType.ToString() // Zamiana enuma na string/wyświetlanie
            ))
            .FirstOrDefaultAsync();
    }

    public async Task<GroupFormDto?> GetFormByIdAsync(int id)
    {
        return await context.Groups
            .AsNoTracking()
            .Where(g => g.Id == id)
            .Select(g => new GroupFormDto
            {
                Id = g.Id,
                Name = g.Name,
                SemesterId = g.SemesterId,
                FieldOfStudyId = g.FieldOfStudyId,
                SpecializationId = g.SpecializationId,
                ClassType = g.ClassType
            })
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(GroupFormDto dto)
    {
        var group = new Group
        {
            Name = dto.Name.Trim(),
            SemesterId = dto.SemesterId,
            FieldOfStudyId = dto.FieldOfStudyId,
            SpecializationId = dto.SpecializationId,
            ClassType = dto.ClassType
        };

        context.Groups.Add(group);
        await context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(GroupFormDto dto)
    {
        var group = await context.Groups.FirstOrDefaultAsync(g => g.Id == dto.Id);
        if (group == null)
        {
            return false;
        }

        group.Name = dto.Name.Trim();
        group.SemesterId = dto.SemesterId;
        group.FieldOfStudyId = dto.FieldOfStudyId;
        group.SpecializationId = dto.SpecializationId;
        group.ClassType = dto.ClassType;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var group = await context.Groups.FirstOrDefaultAsync(g => g.Id == id);
        if (group == null)
        {
            return false;
        }

        context.Groups.Remove(group);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Groups.AnyAsync(e => e.Id == id);
    }

    public async Task<Dictionary<int, string>> GetFieldsOfStudyDropdownListAsync()
    {
        return await context.FieldsOfStudy
            .ToDictionaryAsync(f => f.Id, f => f.Name);
    }

    public async Task<Dictionary<int, string>> GetSemestersDropdownListAsync()
    {
        return await context.Semesters
            .ToDictionaryAsync(s => s.Id, s => s.Name);
    }

    public async Task<Dictionary<int, string>> GetSpecializationsDropdownListAsync()
    {
        return await context.Specializations
            .ToDictionaryAsync(s => s.Id, s => s.Name);
    }

    public async Task<List<GroupAdminItemDto>> GetAllForAdminAsync(
        GroupFilterDto filter)
    {
        var query = context.Groups
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            var normalizedName = filter.Name.Trim().ToLower();

            query = query.Where(group =>
                group.Name.ToLower().Contains(normalizedName));
        }

        if (filter.SemesterId.HasValue)
        {
            query = query.Where(group =>
                group.SemesterId == filter.SemesterId.Value);
        }

        if (filter.FieldOfStudyId.HasValue)
        {
            query = query.Where(group =>
                group.FieldOfStudyId == filter.FieldOfStudyId.Value);
        }

        if (filter.SpecializationId.HasValue)
        {
            query = query.Where(group =>
                group.SpecializationId ==
                filter.SpecializationId.Value);
        }

        if (filter.ClassType.HasValue)
        {
            var classType = (ClassType)filter.ClassType.Value;

            query = query.Where(group =>
                group.ClassType == classType);
        }

        var groups = await query
            .OrderBy(group => group.Name)
            .ThenBy(group => group.Semester!.Name)
            .Select(group => new
            {
                group.Id,
                group.Name,
                group.SemesterId,

                SemesterName = group.Semester != null
                    ? group.Semester.Name
                    : "Brak",

                group.FieldOfStudyId,

                FieldOfStudyName = group.FieldOfStudy != null
                    ? group.FieldOfStudy.Name
                    : "Brak",

                group.SpecializationId,

                SpecializationName = group.Specialization != null
                    ? group.Specialization.Name
                    : null,

                ClassTypeValue = group.ClassType
            })
            .ToListAsync();

        return groups
            .Select(group => new GroupAdminItemDto(
                group.Id,
                group.Name,
                group.SemesterId,
                group.SemesterName,
                group.FieldOfStudyId,
                group.FieldOfStudyName,
                group.SpecializationId,
                group.SpecializationName,
                (int)group.ClassTypeValue,
                GetEnumDisplayName(group.ClassTypeValue)))
            .ToList();
    }

    public async Task<GroupAdminMetadataDto> GetAdminMetadataAsync()
    {
        var nameSuggestions = await context.Groups
            .AsNoTracking()
            .Select(group => group.Name)
            .Distinct()
            .OrderBy(name => name)
            .ToListAsync();

        var semesters = await context.Semesters
            .AsNoTracking()
            .OrderBy(semester => semester.StartDate)
            .ThenBy(semester => semester.Name)
            .Select(semester => new GroupDropdownItemDto(
                semester.Id,
                semester.Name))
            .ToListAsync();

        var fieldsOfStudy = await context.FieldsOfStudy
            .AsNoTracking()
            .OrderBy(field => field.Name)
            .Select(field => new GroupDropdownItemDto(
                field.Id,
                field.Name))
            .ToListAsync();

        var specializations = await context.Specializations
            .AsNoTracking()
            .OrderBy(specialization => specialization.Name)
            .Select(specialization => new GroupDropdownItemDto(
                specialization.Id,
                specialization.Name))
            .ToListAsync();

        var classTypes = Enum
            .GetValues<ClassType>()
            .Select(classType => new GroupDropdownItemDto(
                (int)classType,
                GetEnumDisplayName(classType)))
            .ToList();

        return new GroupAdminMetadataDto
        {
            NameSuggestions = nameSuggestions,
            Semesters = semesters,
            FieldsOfStudy = fieldsOfStudy,
            Specializations = specializations,
            ClassTypes = classTypes
        };
    }

    public async Task<bool> IsValidForeignKeysAsync(
        GroupFormDto dto)
    {
        var semesterExists = await context.Semesters
            .AnyAsync(semester =>
                semester.Id == dto.SemesterId);

        if (!semesterExists)
        {
            return false;
        }

        var fieldExists = await context.FieldsOfStudy
            .AnyAsync(field =>
                field.Id == dto.FieldOfStudyId);

        if (!fieldExists)
        {
            return false;
        }

        if (dto.SpecializationId.HasValue)
        {
            var specializationExists =
                await context.Specializations
                    .AnyAsync(specialization =>
                        specialization.Id ==
                        dto.SpecializationId.Value);

            if (!specializationExists)
            {
                return false;
            }
        }

        return Enum.IsDefined(dto.ClassType);
    }

    private static string GetEnumDisplayName<TEnum>(
        TEnum value)
        where TEnum : struct, Enum
    {
        var member = typeof(TEnum)
            .GetMember(value.ToString())
            .FirstOrDefault();

        var displayAttribute = member?
            .GetCustomAttribute<DisplayAttribute>();

        return displayAttribute?.Name ?? value.ToString();
    }
}