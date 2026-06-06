using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.DTOs.Timetable;
using WebApplication.Models;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services.ModelServices;

public class TimetablesService(AppDbContext context) : ITimetablesService
{
    public async Task<IEnumerable<TimetableListDto>> GetAllWithRelationsAsync()
    {
        return await context.Timetables
            .Select(t => new TimetableListDto
            {
                Id = t.Id,
                ClassType = t.ClassType,
                DayOfWeek = t.DayOfWeek,
                StartTime = t.StartTime,
                EndTime = t.EndTime,
                WeekCycle = t.WeekCycle,
                GroupName = t.Group != null ? t.Group.Name : string.Empty,
                RoomNumber = t.Room != null ? t.Room.RoomNumber : string.Empty,
                SubjectName = t.Subject != null ? t.Subject.Name : string.Empty,
                TeacherName = t.Teacher != null
                    ? $"{(string.IsNullOrEmpty(t.Teacher.AcademicTitle) ? "" : t.Teacher.AcademicTitle + " ")}{t.Teacher.FirstName.Substring(0, 1)}. {t.Teacher.LastName}"
                    : string.Empty
            })
            .ToListAsync();
    }

    public async Task<TimetableDetailsDto?> GetByIdWithRelationsAsync(int id)
    {
        return await context.Timetables
            .Where(m => m.Id == id)
            .Select(t => new TimetableDetailsDto
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
                GroupName = t.Group != null ? t.Group.Name : string.Empty,
                RoomNumber = t.Room != null ? t.Room.RoomNumber : string.Empty,
                SubjectName = t.Subject != null ? t.Subject.Name : string.Empty,
                TeacherName = t.Teacher != null
                    ? $"{(string.IsNullOrEmpty(t.Teacher.AcademicTitle) ? "" : t.Teacher.AcademicTitle + " ")}{t.Teacher.FirstName.Substring(0, 1)}. {t.Teacher.LastName}"
                    : string.Empty
            })
            .FirstOrDefaultAsync();
    }

    public async Task<TimetableEditDto?> GetByIdAsync(int id)
    {
        return await context.Timetables
            .Where(t => t.Id == id)
            .Select(t => new TimetableEditDto
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

    public async Task CreateAsync(TimetableCreateDto dto)
    {
        var timetable = new Timetable
        {
            SubjectId = dto.SubjectId,
            TeacherId = dto.TeacherId,
            RoomId = dto.RoomId,
            GroupId = dto.GroupId,
            ClassType = dto.ClassType,
            DayOfWeek = dto.DayOfWeek,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            WeekCycle = dto.WeekCycle
        };

        context.Add(timetable);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TimetableEditDto dto)
    {
        // Mapowanie DTO -> Encja bazy danych
        var timetable = new Timetable
        {
            Id = dto.Id,
            SubjectId = dto.SubjectId,
            TeacherId = dto.TeacherId,
            RoomId = dto.RoomId,
            GroupId = dto.GroupId,
            ClassType = dto.ClassType,
            DayOfWeek = dto.DayOfWeek,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            WeekCycle = dto.WeekCycle
        };

        context.Update(timetable);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
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
        // Tutaj mapujemy dane bezpośrednio pod listę rozwijaną w formularzu modalnym
        return await context.Teachers
            .Select(t => new Teacher
            {
                Id = t.Id,
                FirstName =
                    $"{(string.IsNullOrEmpty(t.AcademicTitle) ? "" : t.AcademicTitle + " ")}{t.FirstName.Substring(0, 1)}. {t.LastName}"
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<TimetableListDto>> GetFilteredAsync(TimetableFilterDto filter)
    {
        var query = context.Timetables.AsQueryable();

        if (filter.SubjectId.HasValue)
        {
            query = query.Where(t => t.SubjectId == filter.SubjectId);
        }
        if (filter.TeacherId.HasValue)
        {
            query = query.Where(t => t.TeacherId == filter.TeacherId);
        }
        if (filter.RoomId.HasValue)
        {
            query = query.Where(t => t.RoomId == filter.RoomId);
        }
        if (filter.GroupId.HasValue)
        {
            query = query.Where(t => t.GroupId == filter.GroupId);
        }
        if (filter.ClassType.HasValue)
        {
            query = query.Where(t => t.ClassType == filter.ClassType.Value);
        }
        if (filter.DayOfWeek.HasValue)
        {
            query = query.Where(t => t.DayOfWeek == filter.DayOfWeek.Value);
        }
        if (filter.WeekCycle.HasValue)
        {
            query = query.Where(t => t.WeekCycle == filter.WeekCycle.Value);
        }

        return await query
            .OrderBy(t => t.DayOfWeek)
            .ThenBy(t => t.StartTime)
            .Select(t => new TimetableListDto
            {
                Id = t.Id,
                ClassType = t.ClassType,
                DayOfWeek = t.DayOfWeek,
                StartTime = t.StartTime,
                EndTime = t.EndTime,
                WeekCycle = t.WeekCycle,
                GroupName = t.Group != null ? t.Group.Name : string.Empty,
                RoomNumber = t.Room != null ? t.Room.RoomNumber : string.Empty,
                SubjectName = t.Subject != null ? t.Subject.Name : string.Empty,
                TeacherName = t.Teacher != null
                    ? $"{(string.IsNullOrEmpty(t.Teacher.AcademicTitle) ? "" : t.Teacher.AcademicTitle + " ")}{t.Teacher.FirstName.Substring(0, 1)}. {t.Teacher.LastName}"
                    : string.Empty
            })
            .ToListAsync();
    }
}