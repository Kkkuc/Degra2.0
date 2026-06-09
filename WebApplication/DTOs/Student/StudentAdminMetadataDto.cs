namespace WebApplication.DTOs.Student;

public sealed class StudentAdminMetadataDto
{
    public List<string> StudentSuggestions { get; set; } = [];

    public List<StudentGroupItemDto> Groups { get; set; } = [];
}