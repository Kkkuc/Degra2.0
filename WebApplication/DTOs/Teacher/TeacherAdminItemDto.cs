namespace WebApplication.DTOs.Teacher;

public sealed record TeacherAdminItemDto(
    int Id,
    string AcademicTitle,
    string FirstName,
    string LastName,
    string? Email);