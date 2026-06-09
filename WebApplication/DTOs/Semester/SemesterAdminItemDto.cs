namespace WebApplication.DTOs.Semester;

public sealed record SemesterAdminItemDto(
    int Id,
    string Name,
    int AcademicYearId,
    string AcademicYearName,
    DateOnly StartDate,
    DateOnly EndDate);