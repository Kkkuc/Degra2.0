using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Services;

public class StudentsService(AppDbContext context) : IStudentsService
{
    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        return await context.Students.ToListAsync();
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        return await context.Students.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task CreateAsync(Student student)
    {
        context.Add(student);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Student student)
    {
        context.Update(student);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var student = await context.Students.FindAsync(id);
        if (student != null)
        {
            context.Students.Remove(student);
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Students.AnyAsync(e => e.Id == id);
    }
}