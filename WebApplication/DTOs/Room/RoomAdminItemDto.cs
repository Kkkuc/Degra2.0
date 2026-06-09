namespace WebApplication.DTOs.Room;

public sealed record RoomAdminItemDto(
    int Id,
    string RoomNumber,
    int BuildingId,
    string BuildingName);