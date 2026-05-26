using WebApplication.Models;

namespace WebApplication.Services
{
    public interface ISpecializationsService
    {
        Task<IEnumerable<Specialization>> GetAllAsync();
        Task<Specialization?> GetByIdAsync(int id);
        Task CreateAsync(Specialization specialization);
        Task UpdateAsync(Specialization specialization);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}