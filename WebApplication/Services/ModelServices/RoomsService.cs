using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.DTOs.Room;
using WebApplication.Models;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services.ModelServices;

public class RoomsService(AppDbContext context) : IRoomsService
{
    public async Task<IEnumerable<RoomIndexDto>> GetAllForIndexAsync()
    {
        return await context.Rooms
            .Select(r => new RoomIndexDto(
                r.Id,
                r.RoomNumber,
                r.Capacity,
                r.Building != null ? r.Building.Name : "Undentified"
            ))
            .ToListAsync();
    }

    public async Task<RoomDetailsDto?> GetDetailsByIdAsync(int id)
    {
        return await context.Rooms
            .Where(r => r.Id == id)
            .Select(r => new RoomDetailsDto(
                r.Id,
                r.RoomNumber,
                r.Capacity,
                r.RoomType.ToString(),
                r.Building != null ? r.Building.Name : "Undentified"
            ))
            .FirstOrDefaultAsync();
    }

    public async Task<RoomFormDto?> GetFormByIdAsync(int id)
    {
        return await context.Rooms
            .AsNoTracking()
            .Where(room => room.Id == id)
            .Select(room => new RoomFormDto
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                BuildingId = room.BuildingId
            })
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(RoomFormDto dto)
    {
        var room = new Room
        {
            RoomNumber = dto.RoomNumber.Trim(),
            BuildingId = dto.BuildingId
        };

        context.Rooms.Add(room);
        await context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(RoomFormDto dto)
    {
        var room = await context.Rooms
            .FirstOrDefaultAsync(item => item.Id == dto.Id);

        if (room is null)
        {
            return false;
        }

        room.RoomNumber = dto.RoomNumber.Trim();
        room.BuildingId = dto.BuildingId;

        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var room = await context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
        if (room == null)
        {
            return false;
        }

        context.Rooms.Remove(room);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Rooms.AnyAsync(e => e.Id == id);
    }

    public async Task<Dictionary<int, string>> GetBuildingsDropdownListAsync()
    {
        return await context.Buildings
            .ToDictionaryAsync(b => b.Id, b => b.Name);
    }
    
    public async Task<List<RoomPublicDto>> GetPublicListAsync()
    {
        return await context.Rooms
            .AsNoTracking()
            .OrderBy(room => room.Building!.Name)
            .ThenBy(room => room.RoomNumber)
            .Select(room => new RoomPublicDto(
                room.Id,
                room.RoomNumber,
                room.Building != null
                    ? room.Building.Name
                    : "Brak budynku"))
            .ToListAsync();
    }
    
    public async Task<List<RoomAdminItemDto>> GetAllForAdminAsync(
        RoomAdminFilterDto filter)
    {
        var query = context.Rooms
            .AsNoTracking()
            .AsQueryable();

        if (filter.RoomId.HasValue)
        {
            query = query.Where(room =>
                room.Id == filter.RoomId.Value);
        }
        else if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search
                .Trim()
                .ToLower();

            query = query.Where(room =>
                room.RoomNumber.ToLower().Contains(search) ||
                (
                    room.Building != null &&
                    room.Building.Name.ToLower().Contains(search)
                ));
        }

        if (filter.BuildingId.HasValue)
        {
            query = query.Where(room =>
                room.BuildingId == filter.BuildingId.Value);
        }

        return await query
            .OrderBy(room => room.Building!.Name)
            .ThenBy(room => room.RoomNumber)
            .Select(room => new RoomAdminItemDto(
                room.Id,
                room.RoomNumber,
                room.BuildingId,
                room.Building != null
                    ? room.Building.Name
                    : "Brak budynku"))
            .ToListAsync();
    }
    
    public async Task<RoomAdminMetadataDto> GetAdminMetadataAsync()
    {
        var rooms = await context.Rooms
            .AsNoTracking()
            .OrderBy(room => room.Building!.Name)
            .ThenBy(room => room.RoomNumber)
            .Select(room => new
            {
                room.Id,
                room.RoomNumber,

                BuildingName = room.Building != null
                    ? room.Building.Name
                    : "Brak budynku"
            })
            .ToListAsync();

        var buildings = await context.Buildings
            .AsNoTracking()
            .OrderBy(building => building.Name)
            .Select(building => new RoomBuildingItemDto(
                building.Id,
                building.Name))
            .ToListAsync();

        return new RoomAdminMetadataDto
        {
            RoomSuggestions = rooms
                .Select(room => new RoomSuggestionDto(
                    room.Id,
                    $"{room.RoomNumber} — {room.BuildingName}"))
                .ToList(),

            Buildings = buildings
        };
    }
    
    public async Task<bool> BuildingExistsAsync(int buildingId)
    {
        return await context.Buildings
            .AnyAsync(building => building.Id == buildingId);
    }
    
    public async Task<bool> RoomNumberExistsAsync(
        string roomNumber,
        int buildingId,
        int? excludedRoomId = null)
    {
        var normalizedNumber = roomNumber
            .Trim()
            .ToLower();

        return await context.Rooms
            .AnyAsync(room =>
                room.BuildingId == buildingId &&
                room.RoomNumber.ToLower() == normalizedNumber &&
                (
                    !excludedRoomId.HasValue ||
                    room.Id != excludedRoomId.Value
                ));
    }
    
    
}