namespace WebApplication.DTOs.Semester;

public sealed class SemesterAdminMetadataDto
{
    public List<string> NameSuggestions { get; set; } = [];

    public List<SemesterDropdownItemDto> AcademicYears { get; set; } = [];
}