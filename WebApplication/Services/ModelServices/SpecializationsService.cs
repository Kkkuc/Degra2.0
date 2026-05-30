using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.DTOs.Specialization;
using WebApplication.Models;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services.ModelServices;

public class SpecializationsService(AppDbContext context) : ISpecializationsService
{
    public async Task<IEnumerable<SpecializationDto>> GetAllAsync()
    {
        return await context.Specializations
            .Select(s => new SpecializationDto(s.Id, s.Name))
            .ToListAsync();
    }
    
    public async Task<SpecializationDto?> GetByIdAsync(int id)
    {
        return await context.Specializations
            .Where(s => s.Id == id)
            .Select(s => new SpecializationDto(s.Id, s.Name))
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(SpecializationDto dto)
    {
        var specialization = new Specialization
        {
            Name = dto.Name
        };

        context.Specializations.Add(specialization);
        await context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(SpecializationDto dto)
    {
        var specialization = await context.Specializations.FirstOrDefaultAsync(s => s.Id == dto.Id);
        if (specialization == null)
        {
            return false;
        }

        specialization.Name = dto.Name;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var specialization = await context.Specializations.FirstOrDefaultAsync(s => s.Id == id);
        if (specialization == null)
        {
            return false;
        }

        context.Specializations.Remove(specialization);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Specializations.AnyAsync(e => e.Id == id);
    }
}