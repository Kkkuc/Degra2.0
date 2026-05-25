using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services.ModelServices;

public class GroupsService(AppDbContext context) : IGroupsService
{
    public async Task<IEnumerable<Group>> GetAllWithRelationsAsync()
    {
        return await context.Groups
            .Include(g => g.FieldOfStudy)
            .Include(g => g.Semester)
            .Include(g => g.Specialization)
            .ToListAsync();
    }

    public async Task<Group?> GetByIdWithRelationsAsync(int id)
    {
        return await context.Groups
            .Include(g => g.FieldOfStudy)
            .Include(g => g.Semester)
            .Include(g => g.Specialization)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Group?> GetByIdAsync(int id)
    {
        return await context.Groups.FindAsync(id);
    }

    public async Task CreateAsync(Group group)
    {
        context.Add(group);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Group group)
    {
        context.Update(group);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var group = await context.Groups.FindAsync(id);
        if (group != null)
        {
            context.Groups.Remove(group);
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Groups.AnyAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<FieldOfStudy>> GetAllFieldsOfStudyAsync()
    {
        return await context.FieldsOfStudy.ToListAsync();
    }

    public async Task<IEnumerable<Semester>> GetAllSemestersAsync()
    {
        return await context.Semesters.ToListAsync();
    }

    public async Task<IEnumerable<Specialization>> GetAllSpecializationsAsync()
    {
        return await context.Specializations.ToListAsync();
    }
}