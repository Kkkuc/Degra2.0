using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Services;

public class TeachersService(AppDbContext context) : ITeachersService
{
    public async Task<IEnumerable<Teacher>> GetAllAsync()
    {
        return await context.Teachers.ToListAsync();
    }

    public async Task<Teacher?> GetByIdAsync(int id)
    {
        return await context.Teachers.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task CreateAsync(Teacher teacher)
    {
        context.Add(teacher);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Teacher teacher)
    {
        context.Update(teacher);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var teacher = await context.Teachers.FindAsync(id);
        if (teacher != null)
        {
            context.Teachers.Remove(teacher);
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Teachers.AnyAsync(e => e.Id == id);
    }
}