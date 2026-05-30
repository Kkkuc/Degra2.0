using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Services.ModelServices;

public class TimetablesService(AppDbContext context) : ITimetablesService
{
    public async Task<IEnumerable<Timetable>> GetAllWithRelationsAsync()
    {
        return await context.Timetables
            .Include(t => t.Group)
            .Include(t => t.Room)
            .Include(t => t.Subject)
            .Include(t => t.Teacher)
            .ToListAsync();
    }

    public async Task<Timetable?> GetByIdWithRelationsAsync(int id)
    {
        return await context.Timetables
            .Include(t => t.Group)
            .Include(t => t.Room)
            .Include(t => t.Subject)
            .Include(t => t.Teacher)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Timetable?> GetByIdAsync(int id)
    {
        return await context.Timetables.FindAsync(id);
    }

    public async Task CreateAsync(Timetable timetable)
    {
        context.Add(timetable);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Timetable timetable)
    {
        context.Update(timetable);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var timetable = await context.Timetables.FindAsync(id);
        if (timetable != null)
        {
            context.Timetables.Remove(timetable);
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Timetables.AnyAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Group>> GetAllGroupsAsync()
    {
        return await context.Groups.ToListAsync();
    }

    public async Task<IEnumerable<Room>> GetAllRoomsAsync()
    {
        return await context.Rooms.ToListAsync();
    }

    public async Task<IEnumerable<Subject>> GetAllSubjectsAsync()
    {
        return await context.Subjects.ToListAsync();
    }

    public async Task<IEnumerable<Teacher>> GetAllTeachersAsync()
    {
        return await context.Teachers.ToListAsync();
    }
}