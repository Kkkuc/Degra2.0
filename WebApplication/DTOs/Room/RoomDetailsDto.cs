namespace WebApplication.DTOs.Room;

public record RoomDetailsDto(
    int Id,
    string RoomNumber,
    int? Capacity,
    string RoomTypeDisplay,
    string BuildingName);