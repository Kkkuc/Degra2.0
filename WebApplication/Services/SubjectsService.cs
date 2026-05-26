using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Services;

public class SubjectsService(AppDbContext context) : ISubjectsService
{
    public async Task<IEnumerable<Subject>> GetAllAsync()
    {
        return await context.Subjects.ToListAsync();
    }

    public async Task<Subject?> GetByIdAsync(int id)
    {
        return await context.Subjects.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task CreateAsync(Subject subject)
    {
        context.Add(subject);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Subject subject)
    {
        context.Update(subject);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var subject = await context.Subjects.FindAsync(id);
        if (subject != null)
        {
            context.Subjects.Remove(subject);
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Subjects.AnyAsync(e => e.Id == id);
    }
}