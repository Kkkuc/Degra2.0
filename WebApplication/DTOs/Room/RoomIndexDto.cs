namespace WebApplication.DTOs.Room;

public record RoomIndexDto(
    int Id,
    string RoomNumber,
    int? Capacity,
    string BuildingName);