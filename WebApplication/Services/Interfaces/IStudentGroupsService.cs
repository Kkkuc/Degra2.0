using WebApplication.Models;

namespace WebApplication.Services;

// Prosta struktura pomocnicza do ładnego wyświetlania studenta w dropdownie
public class StudentLookupItem { public int Id { get; set; } public string FullName { get; set; } = string.Empty; }

public interface IStudentGroupsService
{
    Task<IEnumerable<StudentGroup>> GetAllWithRelationsAsync();
    Task<StudentGroup?> GetByStudentIdWithRelationsAsync(int studentId);
    Task<StudentGroup?> GetByStudentIdAsync(int studentId);
    Task CreateAsync(StudentGroup studentGroup);
    Task UpdateAsync(StudentGroup studentGroup);
    Task DeleteAsync(int studentId);
    Task<bool> ExistsAsync(int studentId);

    // Metody ładujące dane do SelectListów
    Task<IEnumerable<Group>> GetAllGroupsAsync();
    Task<IEnumerable<StudentLookupItem>> GetStudentsLookupAsync();
}