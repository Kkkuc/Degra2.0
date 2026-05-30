using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.DTOs.Room;
using WebApplication.Models;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services
{
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
                .Where(r => r.Id == id)
                .Select(r => new RoomFormDto
                {
                    Id = r.Id,
                    BuildingId = r.BuildingId,
                    RoomNumber = r.RoomNumber,
                    Capacity = r.Capacity,
                    RoomType = r.RoomType
                })
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(RoomFormDto dto)
        {
            var room = new Room
            {
                BuildingId = dto.BuildingId,
                RoomNumber = dto.RoomNumber,
                Capacity = dto.Capacity,
                RoomType = dto.RoomType
            };

            context.Rooms.Add(room);
            await context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(RoomFormDto dto)
        {
            var room = await context.Rooms.FirstOrDefaultAsync(r => r.Id == dto.Id);
            if (room == null)
            {
                return false;
            }

            room.BuildingId = dto.BuildingId;
            room.RoomNumber = dto.RoomNumber;
            room.Capacity = dto.Capacity;
            room.RoomType = dto.RoomType;

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
    }
}