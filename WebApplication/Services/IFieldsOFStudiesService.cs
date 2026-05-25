using WebApplication.Models;

namespace WebApplication.Services;

public interface IFieldsOfStudiesService
{
    Task<IEnumerable<FieldOfStudy>> GetAllWithFacultyAsync();
    Task<FieldOfStudy?> GetByIdWithFacultyAsync(int id);
    Task<FieldOfStudy?> GetByIdAsync(int id);
    Task CreateAsync(FieldOfStudy fieldOfStudy);
    Task UpdateAsync(FieldOfStudy fieldOfStudy);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
        
    // Pomocnicza metoda dla listy rozwijanej wydziałów w widokach Create/Edit
    Task<IEnumerable<Faculty>> GetAllFacultiesAsync();
}