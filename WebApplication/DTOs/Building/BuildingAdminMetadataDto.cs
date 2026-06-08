namespace WebApplication.DTOs.Building;

public record BuildingFilterOptionDto(int Id, string Text);

public record BuildingAdminMetadataDto(
    IEnumerable<BuildingFilterOptionDto> Faculties,
    IEnumerable<string> NameSuggestions,
    IEnumerable<BuildingFilterOptionDto> AddressSuggestions);
