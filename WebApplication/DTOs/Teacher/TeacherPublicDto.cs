namespace WebApplication.DTOs.Teacher;

public sealed record TeacherPublicDto(
    int Id,
    string AcademicTitle,
    string FirstName,
    string LastName,
    string? Email)
{
    public string FullName =>
        string.Join(
            " ",
            new[]
            {
                AcademicTitle,
                FirstName,
                LastName
            }.Where(value =>
                !string.IsNullOrWhiteSpace(value)));
}