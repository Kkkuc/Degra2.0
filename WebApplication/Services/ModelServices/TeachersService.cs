using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.DTOs.Teacher;
using WebApplication.Models;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services.ModelServices;

public class TeachersService(AppDbContext context) : ITeachersService
{
    public async Task<IEnumerable<TeacherDto>> GetAllAsync()
    {
        return await context.Teachers
            .Select(t => new TeacherDto(
                t.Id,
                t.FirstName + " " + t.LastName,
                !string.IsNullOrEmpty(t.AcademicTitle) ? t.AcademicTitle + " " : "",
                t.Email ?? "Brak"
            ))
            .ToListAsync();
    }

    public async Task<TeacherDto?> GetByIdAsync(int id)
    {
        return await context.Teachers
            .Where(t => t.Id == id)
            .Select(t => new TeacherDto(
                t.Id,
                t.FirstName + " " + t.LastName,
                !string.IsNullOrEmpty(t.AcademicTitle) ? t.AcademicTitle + " " : "",
                t.Email ?? "Brak"
            ))
            .FirstOrDefaultAsync();
    }

    public async Task<TeacherFormDto?> GetFormByIdAsync(int id)
    {
        return await context.Teachers
            .AsNoTracking()
            .Where(teacher => teacher.Id == id)
            .Select(teacher => new TeacherFormDto
            {
                Id = teacher.Id,
                AcademicTitle = teacher.AcademicTitle,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                Email = teacher.Email
            })
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(TeacherFormDto dto)
    {
        var teacher = new Teacher
        {
            AcademicTitle =
                string.IsNullOrWhiteSpace(dto.AcademicTitle)
                    ? null
                    : dto.AcademicTitle.Trim(),

            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),

            Email =
                string.IsNullOrWhiteSpace(dto.Email)
                    ? null
                    : dto.Email.Trim().ToLowerInvariant()
        };

        context.Teachers.Add(teacher);
        await context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(TeacherFormDto dto)
    {
        var teacher = await context.Teachers
            .FirstOrDefaultAsync(item =>
                item.Id == dto.Id);

        if (teacher is null)
        {
            return false;
        }

        teacher.AcademicTitle =
            string.IsNullOrWhiteSpace(dto.AcademicTitle)
                ? null
                : dto.AcademicTitle.Trim();

        teacher.FirstName = dto.FirstName.Trim();
        teacher.LastName = dto.LastName.Trim();

        teacher.Email =
            string.IsNullOrWhiteSpace(dto.Email)
                ? null
                : dto.Email.Trim().ToLowerInvariant();

        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var teacher = await context.Teachers.FirstOrDefaultAsync(t => t.Id == id);
        if (teacher == null) return false;

        context.Teachers.Remove(teacher);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Teachers.AnyAsync(e => e.Id == id);
    }

    public async Task<List<TeacherPublicDto>> GetPublicListAsync()
    {
        return await context.Teachers
            .AsNoTracking()
            .OrderBy(teacher => teacher.LastName)
            .ThenBy(teacher => teacher.FirstName)
            .Select(teacher => new TeacherPublicDto(
                teacher.Id,
                teacher.AcademicTitle ?? string.Empty,
                teacher.FirstName,
                teacher.LastName,
                teacher.Email))
            .ToListAsync();
    }

    public async Task<List<TeacherAdminItemDto>>
        GetAllForAdminAsync(
            TeacherAdminFilterDto filter)
    {
        var query = context.Teachers
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search
                .Trim()
                .ToLower();

            query = query.Where(teacher =>
                teacher.FirstName.ToLower().Contains(search) ||
                teacher.LastName.ToLower().Contains(search) ||
                (
                    teacher.AcademicTitle != null &&
                    teacher.AcademicTitle
                        .ToLower()
                        .Contains(search)
                ) ||
                (
                    teacher.Email != null &&
                    teacher.Email
                        .ToLower()
                        .Contains(search)
                ));
        }

        return await query
            .OrderBy(teacher => teacher.LastName)
            .ThenBy(teacher => teacher.FirstName)
            .Select(teacher => new TeacherAdminItemDto(
                teacher.Id,
                teacher.AcademicTitle ?? string.Empty,
                teacher.FirstName,
                teacher.LastName,
                teacher.Email))
            .ToListAsync();
    }

    public async Task<TeacherAdminMetadataDto>
        GetAdminMetadataAsync()
    {
        var teachers = await context.Teachers
            .AsNoTracking()
            .OrderBy(teacher => teacher.LastName)
            .ThenBy(teacher => teacher.FirstName)
            .Select(teacher => new
            {
                teacher.AcademicTitle,
                teacher.FirstName,
                teacher.LastName
            })
            .ToListAsync();

        var suggestions = teachers
            .Select(teacher =>
                string.Join(
                    " ",
                    new[]
                    {
                        teacher.AcademicTitle,
                        teacher.FirstName,
                        teacher.LastName
                    }.Where(value =>
                        !string.IsNullOrWhiteSpace(value))))
            .Distinct()
            .ToList();

        return new TeacherAdminMetadataDto
        {
            Suggestions = suggestions
        };
    }

    public async Task<bool> EmailExistsAsync(
        string email,
        int? excludedTeacherId = null)
    {
        var normalizedEmail = email
            .Trim()
            .ToLower();

        return await context.Teachers
            .AnyAsync(teacher =>
                teacher.Email != null &&
                teacher.Email.ToLower() == normalizedEmail &&
                (
                    !excludedTeacherId.HasValue ||
                    teacher.Id != excludedTeacherId.Value
                ));
    }
}