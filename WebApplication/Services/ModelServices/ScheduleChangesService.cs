using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.DTOs.ScheduleChange;
using WebApplication.Models;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services.ModelServices;

public class ScheduleChangesService(AppDbContext context) : IScheduleChangesService
{
    public async Task<IEnumerable<ScheduleChangeIndexDto>> GetAllForIndexAsync()
    {
        var rawData = await context.ScheduleChanges
            .Select(s => new
            {
                s.Id,
                s.ChangeDate,
                SubjectName = s.OriginalEntry != null && s.OriginalEntry.Subject != null
                    ? s.OriginalEntry.Subject.Name
                    : "Brak",
                TeacherName = s.OriginalEntry != null && s.OriginalEntry.Teacher != null
                    ? s.OriginalEntry.Teacher.LastName
                    : "Brak",
                DayName = s.OriginalEntry != null ? s.OriginalEntry.DayOfWeek.ToString() : "",
                NewRoom = s.NewRoom != null ? s.NewRoom.RoomNumber : "Bez zmian",
            })
            .ToListAsync();

        return rawData.Select(x => new ScheduleChangeIndexDto(
            x.Id,
            $"{x.SubjectName} | {x.TeacherName} ({x.DayName})",
            x.ChangeDate,
            x.NewRoom
        ));
    }

    public async Task<ScheduleChangeDetailsDto?> GetDetailsByIdAsync(int id)
    {
        var raw = await context.ScheduleChanges
            .Where(s => s.Id == id)
            .Select(s => new
            {
                s.Id,
                s.ChangeDate,
                SubjectName = s.OriginalEntry != null && s.OriginalEntry.Subject != null
                    ? s.OriginalEntry.Subject.Name
                    : "Brak",
                TeacherName = s.OriginalEntry != null && s.OriginalEntry.Teacher != null
                    ? s.OriginalEntry.Teacher.LastName
                    : "Brak",
                DayName = s.OriginalEntry != null ? s.OriginalEntry.DayOfWeek.ToString() : "",
                NewTeacher = s.NewTeacher != null ? s.NewTeacher.FirstName + " " + s.NewTeacher.LastName : "Bez zmian",
                NewRoom = s.NewRoom != null ? s.NewRoom.RoomNumber : "Bez zmian",
                s.NewStartTime,
                s.NewEndTime
            })
            .FirstOrDefaultAsync();

        if (raw == null)
        {
            return null;
        }

        return new ScheduleChangeDetailsDto(
            raw.Id,
            raw.ChangeDate,
            $"{raw.SubjectName} | {raw.TeacherName} ({raw.DayName})",
            raw.NewTeacher,
            raw.NewRoom,
            raw.NewStartTime,
            raw.NewEndTime
        );
    }

    public async Task<ScheduleChangeFormDto?> GetFormByIdAsync(int id)
    {
        return await context.ScheduleChanges
            .Where(s => s.Id == id)
            .Select(s => new ScheduleChangeFormDto
            {
                Id = s.Id,
                TimetableId = s.TimetableId,
                ChangeDate = s.ChangeDate,
                NewRoomId = s.NewRoomId,
                NewTeacherId = s.NewTeacherId,
                NewStartTime = s.NewStartTime,
                NewEndTime = s.NewEndTime
            })
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(ScheduleChangeFormDto dto)
    {
        var change = new ScheduleChange
        {
            TimetableId = dto.TimetableId,
            ChangeDate = dto.ChangeDate,
            NewRoomId = dto.NewRoomId,
            NewTeacherId = dto.NewTeacherId,
            NewStartTime = dto.NewStartTime,
            NewEndTime = dto.NewEndTime
        };
        context.ScheduleChanges.Add(change);
        await context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(ScheduleChangeFormDto dto)
    {
        var change = await context.ScheduleChanges.FirstOrDefaultAsync(s => s.Id == dto.Id);
        if (change == null)
        {
            return false;
        }

        change.TimetableId = dto.TimetableId;
        change.ChangeDate = dto.ChangeDate;
        change.NewRoomId = dto.NewRoomId;
        change.NewTeacherId = dto.NewTeacherId;
        change.NewStartTime = dto.NewStartTime;
        change.NewEndTime = dto.NewEndTime;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var change = await context.ScheduleChanges.FirstOrDefaultAsync(s => s.Id == id);
        if (change == null) return false;

        context.ScheduleChanges.Remove(change);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.ScheduleChanges.AnyAsync(e => e.Id == id);
    }

    public async Task<Dictionary<int, string>> GetRoomsDropdownAsync()
    {
        return await context.Rooms.ToDictionaryAsync(r => r.Id, r => r.RoomNumber);
    }

    public async Task<Dictionary<int, string>> GetTeachersDropdownAsync()
    {
        return await context.Teachers
            .ToDictionaryAsync(t => t.Id, t => t.FirstName + " " + t.LastName);
    }

    public async Task<Dictionary<int, string>> GetTimetablesDropdownAsync()
    {
        var items = await context.Timetables
            .Select(t => new
            {
                t.Id,
                SubjectName = t.Subject != null ? t.Subject.Name : "Brak",
                TeacherName = t.Teacher != null ? t.Teacher.LastName : "Brak",
                t.DayOfWeek,
                t.StartTime
            })
            .ToListAsync();

        return items.ToDictionary(
            t => t.Id,
            t => $"{t.SubjectName} | {t.TeacherName} | {t.DayOfWeek} {t.StartTime:hh\\:mm}"
        );
    }
}