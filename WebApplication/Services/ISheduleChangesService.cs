using WebApplication.Models;

namespace WebApplication.Services
{
    public class TeacherLookupItem { public int Id { get; set; } public string FullName { get; set; } = string.Empty; }
    public class TimetableLookupItem { public int Id { get; set; } public string Text { get; set; } = string.Empty; }

    public interface IScheduleChangesService
    {
        Task<IEnumerable<ScheduleChange>> GetAllWithRelationsAsync();
        Task<ScheduleChange?> GetByIdWithRelationsAsync(int id);
        Task<ScheduleChange?> GetByIdAsync(int id);
        Task CreateAsync(ScheduleChange scheduleChange);
        Task UpdateAsync(ScheduleChange scheduleChange);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        // Metody zasilające listy rozwijane
        Task<IEnumerable<Room>> GetAllRoomsAsync();
        Task<IEnumerable<TeacherLookupItem>> GetTeachersLookupAsync();
        Task<IEnumerable<TimetableLookupItem>> GetTimetablesLookupAsync();
    }
}