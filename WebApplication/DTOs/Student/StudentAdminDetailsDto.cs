namespace WebApplication.DTOs.Student;

public sealed class StudentAdminDetailsDto
{
    public int Id { get; set; }

    public string StudentId { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public List<int> GroupIds { get; set; } = [];

    public List<StudentGroupItemDto> Groups { get; set; } = [];
}