using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.DTOs.Student;
using WebApplication.Models;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services.ModelServices;

public class StudentsService(AppDbContext context) : IStudentsService
{
    public async Task<IEnumerable<StudentDto>> GetAllAsync()
    {
        return await context.Students
            .Select(s => new StudentDto(s.Id, s.FirstName + " " + s.LastName))
            .ToListAsync();
    }

    public async Task<StudentDto?> GetByIdAsync(int id)
    {
        return await context.Students
            .Where(s => s.Id == id)
            .Select(s => new StudentDto(s.Id, s.FirstName + " " + s.LastName))
            .FirstOrDefaultAsync();
    }

    public async Task<StudentDetailsDto?> GetDetailsByIdAsync(int id)
    {
        return await context.Students
            .Where(s => s.Id == id)
            .Select(s => new StudentDetailsDto(s.Id, s.FirstName, s.LastName, s.StudentID))
            .FirstOrDefaultAsync();
    }

    public async Task<StudentFormDto?> GetFormByIdAsync(int id)
    {
        return await context.Students
            .Where(s => s.Id == id)
            .Select(s => new StudentFormDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                StudentId = s.StudentID
            })
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(StudentFormDto dto)
    {
        var student = new Student
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            StudentID = dto.StudentId
        };

        context.Students.Add(student);
        await context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(StudentFormDto dto)
    {
        var student = await context.Students.FirstOrDefaultAsync(s => s.Id == dto.Id);
        if (student == null) return false;

        student.FirstName = dto.FirstName;
        student.LastName = dto.LastName;
        student.StudentID = dto.StudentId;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var student = await context.Students
            .Include(item => item.StudentGroups)
            .FirstOrDefaultAsync(item =>
                item.Id == id);

        if (student is null)
        {
            return false;
        }

        if (student.StudentGroups is not null)
        {
            context.StudentGroups.RemoveRange(
                student.StudentGroups);
        }

        context.Students.Remove(student);

        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Students.AnyAsync(e => e.Id == id);
    }
    
  
    public async Task<StudentAdminDetailsDto?> GetAdminDetailsAsync(
    int id)
{
    return await context.Students
        .AsNoTracking()
        .Where(student => student.Id == id)
        .Select(student => new StudentAdminDetailsDto
        {
            Id = student.Id,
            StudentId = student.StudentID,
            FirstName = student.FirstName,
            LastName = student.LastName,

            GroupIds = student.StudentGroups != null
                ? student.StudentGroups
                    .Select(studentGroup => studentGroup.GroupId)
                    .ToList()
                : new List<int>(),

            Groups = student.StudentGroups != null
                ? student.StudentGroups
                    .OrderBy(studentGroup =>
                        studentGroup.Group!.Name)
                    .Select(studentGroup =>
                        new StudentGroupItemDto(
                            studentGroup.GroupId,
                            studentGroup.Group != null
                                ? studentGroup.Group.Name
                                : "Brak",

                            studentGroup.Group != null &&
                            studentGroup.Group.Semester != null
                                ? studentGroup.Group.Semester.Name
                                : "Brak",

                            studentGroup.Group != null &&
                            studentGroup.Group.FieldOfStudy != null
                                ? studentGroup.Group.FieldOfStudy.Name
                                : "Brak",

                            studentGroup.Group != null &&
                            studentGroup.Group.Specialization != null
                                ? studentGroup.Group.Specialization.Name
                                : null,

                            studentGroup.Group != null
                                ? studentGroup.Group.ClassType.ToString()
                                : "Brak"))
                    .ToList()
                : new List<StudentGroupItemDto>()
        })
        .FirstOrDefaultAsync();
}

    public async Task<List<StudentAdminItemDto>> GetAllForAdminAsync(
        StudentAdminFilterDto filter)
    {
        var query = context.Students
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim().ToLower();

            query = query.Where(student =>
                student.FirstName.ToLower().Contains(search) ||
                student.LastName.ToLower().Contains(search) ||
                student.StudentID.ToLower().Contains(search));
        }

        if (filter.GroupId.HasValue)
        {
            query = query.Where(student =>
                student.StudentGroups != null &&
                student.StudentGroups.Any(studentGroup =>
                    studentGroup.GroupId == filter.GroupId.Value));
        }

        return await query
            .OrderBy(student => student.LastName)
            .ThenBy(student => student.FirstName)
            .Select(student => new StudentAdminItemDto(
                student.Id,
                student.StudentID,
                student.FirstName,
                student.LastName,
                student.StudentGroups != null
                    ? student.StudentGroups.Count
                    : 0))
            .ToListAsync();
    }
    
    public async Task<StudentAdminMetadataDto>
        GetAdminMetadataAsync()
    {
        var students = await context.Students
            .AsNoTracking()
            .OrderBy(student => student.LastName)
            .ThenBy(student => student.FirstName)
            .Select(student =>
                student.StudentID + " — " +
                student.FirstName + " " +
                student.LastName)
            .ToListAsync();

        var rawGroups = await context.Groups
            .AsNoTracking()
            .OrderBy(group => group.Name)
            .Select(group => new
            {
                group.Id,
                group.Name,

                SemesterName = group.Semester != null
                    ? group.Semester.Name
                    : "Brak",

                FieldOfStudyName = group.FieldOfStudy != null
                    ? group.FieldOfStudy.Name
                    : "Brak",

                SpecializationName =
                    group.Specialization != null
                        ? group.Specialization.Name
                        : null,

                ClassType = group.ClassType
            })
            .ToListAsync();

        var groups = rawGroups
            .Select(group => new StudentGroupItemDto(
                group.Id,
                group.Name,
                group.SemesterName,
                group.FieldOfStudyName,
                group.SpecializationName,
                group.ClassType.ToString()))
            .ToList();

        return new StudentAdminMetadataDto
        {
            StudentSuggestions = students,
            Groups = groups
        };
    }

    public async Task<bool> UpdateForAdminAsync(
        StudentAdminFormDto dto)
    {
        var student = await context.Students
            .Include(item => item.StudentGroups)
            .FirstOrDefaultAsync(item =>
                item.Id == dto.Id);

        if (student is null)
        {
            return false;
        }

        student.FirstName = dto.FirstName.Trim();
        student.LastName = dto.LastName.Trim();
        student.StudentID = dto.StudentId.Trim();

        var requestedGroupIds = dto.GroupIds
            .Distinct()
            .ToHashSet();

        var currentGroupIds = student.StudentGroups?
                                  .Select(studentGroup => studentGroup.GroupId)
                                  .ToHashSet()
                              ?? [];

        var membershipsToRemove = student.StudentGroups?
                                      .Where(studentGroup =>
                                          !requestedGroupIds.Contains(
                                              studentGroup.GroupId))
                                      .ToList()
                                  ?? [];

        context.StudentGroups.RemoveRange(
            membershipsToRemove);

        var membershipsToAdd = requestedGroupIds
            .Except(currentGroupIds)
            .Select(groupId => new StudentGroup
            {
                StudentId = student.Id,
                GroupId = groupId
            });

        context.StudentGroups.AddRange(
            membershipsToAdd);

        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> StudentNumberExistsAsync(
        string studentId,
        int? excludedStudentId = null)
    {
        var normalizedStudentId =
            studentId.Trim().ToLower();

        return await context.Students.AnyAsync(student =>
            student.StudentID.ToLower() == normalizedStudentId &&
            (!excludedStudentId.HasValue ||
             student.Id != excludedStudentId.Value));
    }
    
    public async Task<bool> GroupsExistAsync(
        IEnumerable<int> groupIds)
    {
        var distinctIds = groupIds
            .Distinct()
            .ToList();

        if (distinctIds.Count == 0)
        {
            return true;
        }

        var existingCount = await context.Groups
            .CountAsync(group =>
                distinctIds.Contains(group.Id));

        return existingCount == distinctIds.Count;
    }
    
    public async Task<bool> CreateForAdminAsync(
        StudentAdminFormDto dto)
    {
        var groupIds = dto.GroupIds
            .Distinct()
            .ToList();

        var student = new Student
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            StudentID = dto.StudentId.Trim(),
            StudentGroups = groupIds
                .Select(groupId => new StudentGroup
                {
                    GroupId = groupId
                })
                .ToList()
        };

        context.Students.Add(student);
        await context.SaveChangesAsync();

        return true;
    }
    
    
}