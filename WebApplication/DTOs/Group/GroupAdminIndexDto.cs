namespace WebApplication.DTOs.Group;

public record GroupAdminItemDto(
    int Id,
    string Name,
    int SemesterId,
    string SemesterName,
    int FieldOfStudyId,
    string FieldOfStudyName,
    int? SpecializationId,
    string? SpecializationName,
    int ClassType,
    string ClassTypeDisplay);