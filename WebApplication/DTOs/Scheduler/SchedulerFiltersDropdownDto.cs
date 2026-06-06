namespace WebApplication.DTOs.Scheduler;

public class SchedulerFiltersDropdownDto
{
    public List<DropdownItemDto> FieldsOfStudy { get; set; } = [];
    public List<DropdownItemDto> Semesters { get; set; } = [];
}