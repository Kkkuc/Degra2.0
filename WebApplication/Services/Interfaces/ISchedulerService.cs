using WebApplication.DTOs.Scheduler;

namespace WebApplication.Services.Interfaces;

public interface ISchedulerService
{
    Task<SchedulerViewModel> GetSchedulerDataAsync(SchedulerFilterDto filter);
    Task<Dictionary<int, string>> GetFieldsOfStudyDropdownAsync();
    Task<Dictionary<int, string>> GetSemestersDropdownAsync();
}