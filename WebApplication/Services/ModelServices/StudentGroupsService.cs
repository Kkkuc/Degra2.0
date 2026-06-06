using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.DTOs.Student;
using WebApplication.DTOs.StudentGroup;
using WebApplication.Models;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services.ModelServices;

public class StudentGroupsService(AppDbContext context) : IStudentGroupsService
{
    public async Task<IEnumerable<StudentGroupDto>> GetAllWithRelationsAsync()
    {
        return await context.StudentGroups
            .Select(sg => new StudentGroupDto(
                sg.StudentId,
                sg.Student != null ? sg.Student.FirstName + " " + sg.Student.LastName : "Brak",
                sg.GroupId,
                sg.Group != null ? sg.Group.Name : "Brak"
            ))
            .ToListAsync();
    }

    public async Task<StudentGroupDto?> GetByStudentIdWithRelationsAsync(int studentId)
    {
        return await context.StudentGroups
            .Where(sg => sg.StudentId == studentId)
            .Select(sg => new StudentGroupDto(
                sg.StudentId,
                sg.Student != null ? sg.Student.FirstName + " " + sg.Student.LastName : "Brak",
                sg.GroupId,
                sg.Group != null ? sg.Group.Name : "Brak"
            ))
            .FirstOrDefaultAsync();
    }

    public async Task<StudentGroupFormDto?> GetFormByStudentIdAsync(int studentId)
    {
        return await context.StudentGroups
            .Where(sg => sg.StudentId == studentId)
            .Select(sg => new StudentGroupFormDto
            {
                StudentId = sg.StudentId,
                GroupId = sg.GroupId
            })
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(StudentGroupFormDto dto)
    {
        var studentGroup = new StudentGroup
        {
            StudentId = dto.StudentId,
            GroupId = dto.GroupId
        };

        context.StudentGroups.Add(studentGroup);
        await context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(int originalStudentId, StudentGroupFormDto dto)
    {
        var studentGroup = await context.StudentGroups
            .FirstOrDefaultAsync(sg => sg.StudentId == originalStudentId);
            
        if (studentGroup == null)
        {
            return false;
        }

        // Jeśli klucz główny tabeli łączącej pozwala na modyfikację:
        context.StudentGroups.Remove(studentGroup); // Najbezpieczniejsza opcja przy tabelach łączących to Re-create
        
        var updatedStudentGroup = new StudentGroup
        {
            StudentId = dto.StudentId,
            GroupId = dto.GroupId
        };
        
        context.StudentGroups.Add(updatedStudentGroup);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int studentId)
    {
        var studentGroup = await context.StudentGroups
            .FirstOrDefaultAsync(sg => sg.StudentId == studentId);
            
        if (studentGroup == null) return false;

        context.StudentGroups.Remove(studentGroup);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int studentId)
    {
        return await context.StudentGroups.AnyAsync(e => e.StudentId == studentId);
    }

    public async Task<Dictionary<int, string>> GetGroupsDropdownAsync()
    {
        return await context.Groups
            .ToDictionaryAsync(g => g.Id, g => g.Name);
    }

    public async Task<IEnumerable<StudentDto>> GetStudentsLookupAsync()
    {
        return await context.Students
            .Select(s => new StudentDto(
                s.Id,
                s.FirstName + " " + s.LastName
            ))
            .ToListAsync();
    }
}