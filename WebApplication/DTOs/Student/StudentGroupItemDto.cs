namespace WebApplication.DTOs.Student;

public sealed record StudentGroupItemDto(
    int Id,
    string Name,
    string SemesterName,
    string FieldOfStudyName,
    string? SpecializationName,
    string ClassType);