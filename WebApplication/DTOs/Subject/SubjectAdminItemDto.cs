namespace WebApplication.DTOs.Subject;

public sealed record SubjectAdminItemDto(
    int Id,
    string Name,
    string? Abbreviation,
    string? Code);