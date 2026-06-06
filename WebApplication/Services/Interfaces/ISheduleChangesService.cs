using WebApplication.DTOs.ScheduleChange;

namespace WebApplication.Services.Interfaces;

public interface IScheduleChangesService
{
    Task<IEnumerable<ScheduleChangeIndexDto>> GetAllForIndexAsync();
    Task<ScheduleChangeDetailsDto?> GetDetailsByIdAsync(int id);
    Task<ScheduleChangeFormDto?> GetFormByIdAsync(int id);
    Task CreateAsync(ScheduleChangeFormDto dto);
    Task<bool> UpdateAsync(ScheduleChangeFormDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    
    // Dropdown lookups sformatowane bezpośrednio do słowników
    Task<Dictionary<int, string>> GetRoomsDropdownAsync();
    Task<Dictionary<int, string>> GetTeachersDropdownAsync();
    Task<Dictionary<int, string>> GetTimetablesDropdownAsync();
}