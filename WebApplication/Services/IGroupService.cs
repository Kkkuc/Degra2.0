using WebApplication.Models;

namespace WebApplication.Services
{
    public interface IGroupsService
    {
        Task<IEnumerable<Group>> GetAllWithRelationsAsync();
        Task<Group?> GetByIdWithRelationsAsync(int id);
        Task<Group?> GetByIdAsync(int id);
        Task CreateAsync(Group group);
        Task UpdateAsync(Group group);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        // Metody pomocnicze do nakarmienia SelectListów w widokach Create/Edit
        Task<IEnumerable<FieldOfStudy>> GetAllFieldsOfStudyAsync();
        Task<IEnumerable<Semester>> GetAllSemestersAsync();
        Task<IEnumerable<Specialization>> GetAllSpecializationsAsync();
    }
}