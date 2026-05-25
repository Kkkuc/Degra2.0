namespace WebApplication.DTOs.Group;

public record GroupIndexDto(
    int Id,
    string Name,
    string SemesterName,
    string FieldOfStudyName,
    string? SpecializationName,
    string ClassTypeDisplay);
