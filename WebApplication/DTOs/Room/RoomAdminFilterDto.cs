namespace WebApplication.DTOs.Room;

public sealed class RoomAdminFilterDto
{
    public string? Search { get; set; }

    public int? RoomId { get; set; }

    public int? BuildingId { get; set; }
}