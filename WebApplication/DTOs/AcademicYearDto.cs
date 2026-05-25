namespace WebApplication.DTOs;

public record AcademicYearDto(int Id, string Name, DateOnly StartDate, DateOnly EndDate)
{
    public string DurationDisplay => $"{StartDate} - {EndDate}";
}