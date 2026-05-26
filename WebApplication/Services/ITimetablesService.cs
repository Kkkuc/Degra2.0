using WebApplication.Models;

namespace WebApplication.Services;

public interface ITimetablesService
{
    Task<IEnumerable<Timetable>> GetAllWithRelationsAsync();
    Task<Timetable?> GetByIdWithRelationsAsync(int id);
    Task<Timetable?> GetByIdAsync(int id);
    Task CreateAsync(Timetable timetable);
    Task UpdateAsync(Timetable timetable);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);

    // Metody zasilające listy rozwijane
    Task<IEnumerable<Group>> GetAllGroupsAsync();
    Task<IEnumerable<Room>> GetAllRoomsAsync();
    Task<IEnumerable<Subject>> GetAllSubjectsAsync();
    Task<IEnumerable<Teacher>> GetAllTeachersAsync();
}