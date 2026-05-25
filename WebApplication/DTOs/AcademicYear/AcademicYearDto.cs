namespace WebApplication.DTOs.AcademicYear;

public record AcademicYearDto(int Id, string Name, DateOnly StartDate, DateOnly EndDate)
{
    public string DurationDisplay => $"{StartDate.Year}/{EndDate.Year}";
}