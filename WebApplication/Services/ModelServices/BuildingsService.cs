using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.DTOs;
using WebApplication.DTOs.Building;
using WebApplication.Models;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services.ModelServices;

public class BuildingsService(AppDbContext context) : IBuildingsService
{
    public async Task<IEnumerable<BuildingIndexDto>> GetAllForIndexAsync()
    {
        return await context.Buildings
            .Select(b => new BuildingIndexDto(
                b.Id,
                b.Name,
                b.Faculty!.Abbreviation 
            ))
            .ToListAsync();
    }

    public async Task<IEnumerable<BuildingAdminItemDto>> GetAllForAdminAsync(string? search = null, int? facultyId = null)
    {
        var query = context.Buildings
            .AsNoTracking()
            .Include(b => b.Faculty)
            .AsQueryable();

        if (facultyId.HasValue)
        {
            query = query.Where(b => b.FacultyId == facultyId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalized = search.Trim().ToLower();
            query = query.Where(b =>
                b.Name.ToLower().Contains(normalized) ||
                b.Street.ToLower().Contains(normalized) ||
                b.HouseNumber.ToLower().Contains(normalized) ||
                b.City.ToLower().Contains(normalized) ||
                b.PostalCode.ToLower().Contains(normalized) ||
                b.Faculty!.Name.ToLower().Contains(normalized) ||
                b.Faculty!.Abbreviation.ToLower().Contains(normalized));
        }

        return await query
            .OrderBy(b => b.Name)
            .Select(b => new BuildingAdminItemDto(
                b.Id,
                b.Name,
                b.FacultyId,
                b.Faculty!.Name,
                b.Faculty!.Abbreviation,
                b.Street,
                b.HouseNumber,
                b.City,
                b.PostalCode))
            .ToListAsync();
    }

    public async Task<BuildingDetailsDto?> GetDetailsByIdAsync(int id)
    {
        return await context.Buildings
            .Where(b => b.Id == id)
            .Select(b => new BuildingDetailsDto(
                b.Id,
                b.Name,
                new AddressDto
                {
                    Street = b.Street,
                    HouseNumber = b.HouseNumber,
                    PostalCode = b.PostalCode,
                    City = b.City
                },
                b.FacultyId,
                b.Faculty!.Name,
                b.Faculty!.Abbreviation
            ))
            .FirstOrDefaultAsync();
    }

    public async Task<BuildingFormDto?> GetFormByIdAsync(int id)
    {
        return await context.Buildings
            .Where(b => b.Id == id)
            .Select(b => new BuildingFormDto
            {
                Id = b.Id,
                Name = b.Name,
                FacultyId = b.FacultyId,
                AddressDto = new AddressDto
                {
                    Street = b.Street,
                    HouseNumber = b.HouseNumber,
                    City = b.City,
                    PostalCode = b.PostalCode
                }
            })
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(BuildingFormDto dto)
    {
        var building = new Building
        {
            Name = dto.Name,
            Street = dto.AddressDto.Street,
            HouseNumber = dto.AddressDto.HouseNumber,
            City = dto.AddressDto.City,
            PostalCode = dto.AddressDto.PostalCode,
            FacultyId = dto.FacultyId
        };

        context.Add(building);
        await context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(BuildingFormDto dto)
    {
        var building = await context.Buildings.FindAsync(dto.Id);
        if (building == null) return false;

        building.Name = dto.Name;
        building.Street = dto.AddressDto.Street;
        building.HouseNumber = dto.AddressDto.HouseNumber;
        building.City = dto.AddressDto.City;
        building.PostalCode = dto.AddressDto.PostalCode;
        building.FacultyId = dto.FacultyId;

        context.Update(building);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task DeleteAsync(int id)
    {
        var building = await context.Buildings.FindAsync(id);
        if (building != null)
        {
            context.Buildings.Remove(building);
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Buildings.AnyAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<KeyValuePair<int, string>>> GetFacultyDropdownListAsync()
    {
        return await context.Faculties
            .Select(f => new KeyValuePair<int, string>(f.Id, f.Abbreviation)) // Przekazujemy skrót do listy rozwijanej
            .ToListAsync();
    }
}
