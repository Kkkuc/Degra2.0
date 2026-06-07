using WebApplication.DTOs.Teacher;
using WebApplication.DTOs.Timetable;
using WebApplication.Models;

namespace WebApplication.Services.Interfaces;

public interface ITimetablesService
{
    Task<IEnumerable<TimetableListDto>> GetAllWithRelationsAsync();
    Task<TimetableDetailsDto?> GetByIdWithRelationsAsync(int id);
    Task<TimetableEditDto?> GetByIdAsync(int id);
    Task CreateAsync(TimetableCreateDto dto);
    Task UpdateAsync(TimetableEditDto dto);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);

    // Metody zasilające listy rozwijane
    Task<IEnumerable<Group>> GetAllGroupsAsync();
    Task<IEnumerable<Room>> GetAllRoomsAsync();
    Task<IEnumerable<Subject>> GetAllSubjectsAsync();
    Task<IEnumerable<TeacherDropdownDto>> GetAllTeachersAsync();
    Task<IEnumerable<TimetableListDto>> GetFilteredAsync(TimetableFilterDto filter);
}