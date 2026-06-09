namespace WebApplication.DTOs.Room;

public sealed class RoomAdminMetadataDto
{
    public List<RoomSuggestionDto> RoomSuggestions { get; set; } = [];

    public List<RoomBuildingItemDto> Buildings { get; set; } = [];
}