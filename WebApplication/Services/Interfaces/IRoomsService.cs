using WebApplication.DTOs.Room;

namespace WebApplication.Services.Interfaces;

public interface IRoomsService
{
    Task<IEnumerable<RoomIndexDto>> GetAllForIndexAsync();
    Task<RoomDetailsDto?> GetDetailsByIdAsync(int id);
    Task<RoomFormDto?> GetFormByIdAsync(int id);
    Task CreateAsync(RoomFormDto dto);
    Task<bool> UpdateAsync(RoomFormDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<Dictionary<int, string>> GetBuildingsDropdownListAsync();
    
    Task<List<RoomPublicDto>> GetPublicListAsync();

    Task<List<RoomAdminItemDto>> GetAllForAdminAsync(
        RoomAdminFilterDto filter);

    Task<RoomAdminMetadataDto> GetAdminMetadataAsync();

    Task<bool> RoomNumberExistsAsync(
        string roomNumber,
        int buildingId,
        int? excludedRoomId = null);

    Task<bool> BuildingExistsAsync(int buildingId);
}