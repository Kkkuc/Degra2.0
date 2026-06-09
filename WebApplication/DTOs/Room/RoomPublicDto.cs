namespace WebApplication.DTOs.Room;

public sealed record RoomPublicDto(
    int Id,
    string RoomNumber,
    string BuildingName);