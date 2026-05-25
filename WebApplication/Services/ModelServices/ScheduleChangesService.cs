using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class ScheduleChangesService(AppDbContext context) : IScheduleChangesService
    {
        public async Task<IEnumerable<ScheduleChange>> GetAllWithRelationsAsync()
        {
            return await context.ScheduleChanges
                .Include(s => s.NewRoom)
                .Include(s => s.NewTeacher)
                .Include(s => s.OriginalEntry)
                    .ThenInclude(t => t!.Subject)
                .Include(s => s.OriginalEntry)
                    .ThenInclude(t => t!.Teacher)
                .ToListAsync();
        }

        public async Task<ScheduleChange?> GetByIdWithRelationsAsync(int id)
        {
            return await context.ScheduleChanges
                .Include(s => s.NewRoom)
                .Include(s => s.NewTeacher)
                .Include(s => s.OriginalEntry)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<ScheduleChange?> GetByIdAsync(int id)
        {
            return await context.ScheduleChanges.FindAsync(id);
        }

        public async Task CreateAsync(ScheduleChange scheduleChange)
        {
            context.Add(scheduleChange);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ScheduleChange scheduleChange)
        {
            context.Update(scheduleChange);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var scheduleChange = await context.ScheduleChanges.FindAsync(id);
            if (scheduleChange != null)
            {
                context.ScheduleChanges.Remove(scheduleChange);
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await context.ScheduleChanges.AnyAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        {
            return await context.Rooms.ToListAsync();
        }

        public async Task<IEnumerable<TeacherLookupItem>> GetTeachersLookupAsync()
        {
            return await context.Teachers
                .Select(t => new TeacherLookupItem
                {
                    Id = t.Id,
                    FullName = t.FirstName + " " + t.LastName
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<TimetableLookupItem>> GetTimetablesLookupAsync()
        {
            var items = await context.Timetables
                .Include(t => t.Subject)
                .Include(t => t.Teacher)
                .ToListAsync();

            // Formatowanie stringa przeniesione z LINQ-to-Entities do pamięci aplikacji 
            // eliminuje błędy translacji specyficznych formatów dat i godzin na SQL
            return items.Select(t => new TimetableLookupItem
            {
                Id = t.Id,
                Text = $"{t.Subject?.Name} | {t.Teacher?.LastName} | {t.DayOfWeek} {t.StartTime:hh\\:mm}"
            });
        }
    }
}