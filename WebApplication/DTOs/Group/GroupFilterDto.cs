namespace WebApplication.DTOs.Group;

public class GroupFilterDto
{
    public string? Name { get; set; }

    public int? SemesterId { get; set; }

    public int? FieldOfStudyId { get; set; }

    public int? SpecializationId { get; set; }

    public int? ClassType { get; set; }
}