using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.DTOs.Specialization;
using WebApplication.Models;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services.ModelServices;

public class SpecializationsService(
    AppDbContext context) : ISpecializationsService
{
    public async Task<List<SpecializationDto>> GetAllAsync(
        SpecializationFilterDto? filter = null)
    {
        var query = context.Specializations
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Name))
        {
            var name = filter.Name.Trim().ToLower();

            query = query.Where(specialization =>
                specialization.Name.ToLower().Contains(name));
        }

        return await query
            .OrderBy(specialization => specialization.Name)
            .Select(specialization => new SpecializationDto(
                specialization.Id,
                specialization.Name))
            .ToListAsync();
    }

    public async Task<SpecializationDto?> GetByIdAsync(int id)
    {
        return await context.Specializations
            .AsNoTracking()
            .Where(specialization =>
                specialization.Id == id)
            .Select(specialization => new SpecializationDto(
                specialization.Id,
                specialization.Name))
            .FirstOrDefaultAsync();
    }

    public async Task<SpecializationMetadataDto>
        GetMetadataAsync()
    {
        var names = await context.Specializations
            .AsNoTracking()
            .Select(specialization => specialization.Name)
            .Distinct()
            .OrderBy(name => name)
            .ToListAsync();

        return new SpecializationMetadataDto
        {
            NameSuggestions = names
        };
    }

    public async Task CreateAsync(SpecializationDto dto)
    {
        var specialization = new Specialization
        {
            Name = dto.Name.Trim()
        };

        context.Specializations.Add(specialization);
        await context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(
        SpecializationDto dto)
    {
        var specialization = await context.Specializations
            .FirstOrDefaultAsync(item =>
                item.Id == dto.Id);

        if (specialization is null)
        {
            return false;
        }

        specialization.Name = dto.Name.Trim();

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var specialization = await context.Specializations
            .FirstOrDefaultAsync(item =>
                item.Id == id);

        if (specialization is null)
        {
            return false;
        }

        context.Specializations.Remove(specialization);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Specializations
            .AnyAsync(item => item.Id == id);
    }

    public async Task<bool> NameExistsAsync(
        string name,
        int? excludedId = null)
    {
        var normalizedName = name.Trim().ToLower();

        return await context.Specializations
            .AnyAsync(item =>
                item.Name.ToLower() == normalizedName &&
                (!excludedId.HasValue ||
                 item.Id != excludedId.Value));
    }
}