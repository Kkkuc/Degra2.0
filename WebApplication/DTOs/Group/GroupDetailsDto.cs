using WebApplication.Models.enums;

namespace WebApplication.DTOs.Group;

public record GroupDetailsDto(
    int Id,
    string Name,
    string SemesterName,
    string FieldOfStudyName,
    string? SpecializationName,
    string ClassTypeDisplay);
