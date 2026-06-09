namespace WebApplication.DTOs.Student;

public sealed record StudentAdminItemDto(
    int Id,
    string StudentId,
    string FirstName,
    string LastName,
    int GroupsCount);