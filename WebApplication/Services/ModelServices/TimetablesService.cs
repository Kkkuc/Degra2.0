using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Services.ModelServices;

public class TimetablesService(AppDbContext context) : ITimetablesService
{
    public async Task<IEnumerable<Timetable>> GetAllWithRelationsAsync()
    {
        return await context.Timetables
            .Select(t => new Timetable
            {
                Id = t.Id,
                ClassType = t.ClassType,
                DayOfWeek = t.DayOfWeek,
                StartTime = t.StartTime,
                EndTime = t.EndTime,
                WeekCycle = t.WeekCycle,
                
                Group = t.Group != null ? new Group { Id = t.Group.Id, Name = t.Group.Name } : null,
                Room = t.Room != null ? new Room { Id = t.Room.Id, RoomNumber = t.Room.RoomNumber } : null,
                Subject = t.Subject != null ? new Subject { Id = t.Subject.Id, Name = t.Subject.Name } : null,
                Teacher = t.Teacher != null ? new Teacher { Id = t.Teacher.Id, FirstName = t.Teacher.FirstName } : null
            })
            .ToListAsync();
    }   

    public async Task<Timetable?> GetByIdWithRelationsAsync(int id)
    {
        return await context.Timetables
            .Where(m => m.Id == id)
            .Select(t => new Timetable
            {
                Id = t.Id,
                SubjectId = t.SubjectId,
                TeacherId = t.TeacherId,
                RoomId = t.RoomId,
                GroupId = t.GroupId,
                ClassType = t.ClassType,
                DayOfWeek = t.DayOfWeek,
                StartTime = t.StartTime,
                EndTime = t.EndTime,
                WeekCycle = t.WeekCycle,
                Group = t.Group != null ? new Group { Id = t.Group.Id, Name = t.Group.Name } : null,
                Room = t.Room != null ? new Room { Id = t.Room.Id, RoomNumber = t.Room.RoomNumber } : null,
                Subject = t.Subject != null ? new Subject { Id = t.Subject.Id, Name = t.Subject.Name } : null,
                Teacher = t.Teacher != null ? new Teacher { Id = t.Teacher.Id, FirstName = t.Teacher.FirstName } : null
            })
            .FirstOrDefaultAsync();
    }

    public async Task<Timetable?> GetByIdAsync(int id)
    {
        // Zastąpiono FindAsync przez Select, aby pobrać tylko ID i FK potrzebne do formularza edycji
        return await context.Timetables
            .Where(t => t.Id == id)
            .Select(t => new Timetable
            {
                Id = t.Id,
                SubjectId = t.SubjectId,
                TeacherId = t.TeacherId,
                RoomId = t.RoomId,
                GroupId = t.GroupId,
                ClassType = t.ClassType,
                DayOfWeek = t.DayOfWeek,
                StartTime = t.StartTime,
                EndTime = t.EndTime,
                WeekCycle = t.WeekCycle
            })
            .FirstOrDefaultAsync();
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
        // Optymalizacja: Nie pobieramy całego obiektu przed usunięciem. 
        // Tworzymy "pusty" obiekt z samym ID i dołączamy go do śledzenia w celu usunięcia.
        var timetable = new Timetable { Id = id };
        context.Timetables.Entry(timetable).State = EntityState.Deleted;
        await context.SaveChangesAsync();
    }   

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Timetables.AnyAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Group>> GetAllGroupsAsync()
    {
        return await context.Groups
            .Select(g => new Group { Id = g.Id, Name = g.Name })
            .ToListAsync();
    }

    public async Task<IEnumerable<Room>> GetAllRoomsAsync()
    {
        return await context.Rooms
            .Select(r => new Room { Id = r.Id, RoomNumber = r.RoomNumber })
            .ToListAsync();
    }

    public async Task<IEnumerable<Subject>> GetAllSubjectsAsync()
    {
        return await context.Subjects
            .Select(s => new Subject { Id = s.Id, Name = s.Name })
            .ToListAsync();
    }

    public async Task<IEnumerable<Teacher>> GetAllTeachersAsync()
    {
        return await context.Teachers
            .Select(t => new Teacher { Id = t.Id, FirstName = t.FirstName })
            .ToListAsync();
    }
}