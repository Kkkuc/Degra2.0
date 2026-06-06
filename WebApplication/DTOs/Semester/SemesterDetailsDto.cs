namespace WebApplication.DTOs.Semester;

public record SemesterDetailsDto(
    int Id,
    string Name,
    string AcademicYearName,
    DateOnly StartDate,
    DateOnly EndDate);