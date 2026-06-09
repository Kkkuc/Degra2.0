namespace WebApplication.DTOs.Group;

public class GroupAdminMetadataDto
{
    public List<string> NameSuggestions { get; set; } = [];

    public List<GroupDropdownItemDto> Semesters { get; set; } = [];

    public List<GroupDropdownItemDto> FieldsOfStudy { get; set; } = [];

    public List<GroupDropdownItemDto> Specializations { get; set; } = [];

    public List<GroupDropdownItemDto> ClassTypes { get; set; } = [];
}