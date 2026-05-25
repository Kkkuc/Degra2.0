using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Services;

public class StudentGroupsService(AppDbContext context) : IStudentGroupsService
{
    public async Task<IEnumerable<StudentGroup>> GetAllWithRelationsAsync()
    {
        return await context.StudentGroups
            .Include(s => s.Group)
            .Include(s => s.Student)
            .ToListAsync();
    }

    public async Task<StudentGroup?> GetByStudentIdWithRelationsAsync(int studentId)
    {
        return await context.StudentGroups
            .Include(s => s.Group)
            .Include(s => s.Student)
            .FirstOrDefaultAsync(m => m.StudentId == studentId);
    }

    public async Task<StudentGroup?> GetByStudentIdAsync(int studentId)
    {
        return await context.StudentGroups.FindAsync(studentId);
    }

    public async Task CreateAsync(StudentGroup studentGroup)
    {
        context.Add(studentGroup);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(StudentGroup studentGroup)
    {
        context.Update(studentGroup);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int studentId)
    {
        var studentGroup = await context.StudentGroups.FindAsync(studentId);
        if (studentGroup != null)
        {
            context.StudentGroups.Remove(studentGroup);
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int studentId)
    {
        return await context.StudentGroups.AnyAsync(e => e.StudentId == studentId);
    }

    public async Task<IEnumerable<Group>> GetAllGroupsAsync()
    {
        return await context.Groups.ToListAsync();
    }

    public async Task<IEnumerable<StudentLookupItem>> GetStudentsLookupAsync()
    {
        return await context.Students
            .Select(s => new StudentLookupItem
            {
                Id = s.Id,
                FullName = s.FirstName + " " + s.LastName
            })
            .ToListAsync();
    }
}