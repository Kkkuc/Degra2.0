using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.DTOs.Group;
using WebApplication.Models;
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
            Name = dto.Name,
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

        group.Name = dto.Name;
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
}