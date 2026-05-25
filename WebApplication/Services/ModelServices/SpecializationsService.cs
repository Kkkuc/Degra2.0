using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class SpecializationsService(AppDbContext context) : ISpecializationsService
    {
        public async Task<IEnumerable<Specialization>> GetAllAsync()
        {
            return await context.Specializations.ToListAsync();
        }

        public async Task<Specialization?> GetByIdAsync(int id)
        {
            return await context.Specializations.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task CreateAsync(Specialization specialization)
        {
            context.Add(specialization);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Specialization specialization)
        {
            context.Update(specialization);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var specialization = await context.Specializations.FindAsync(id);
            if (specialization != null)
            {
                context.Specializations.Remove(specialization);
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await context.Specializations.AnyAsync(e => e.Id == id);
        }
    }
}