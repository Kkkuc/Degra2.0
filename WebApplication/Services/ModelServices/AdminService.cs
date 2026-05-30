using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.DTOs.Admin;
using WebApplication.Models;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services.ModelServices;

public class AdminService(AppDbContext context) : IAdminService
{
    public async Task<IEnumerable<UserListDto>> GetUsersForIndexAsync()
    {
        return await context.Users
            .Select(u => new UserListDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                RoleName = u.Role != null ? u.Role.Name : "Brak roli"
            })
            .ToListAsync();
    }

    public async Task<Dictionary<int, string>> GetRolesDropdownListAsync()
    {
        return await context.Roles
            .ToDictionaryAsync(r => r.Id, r => r.Name);
    }

    public async Task<bool> UserExistsAsync(string username)
    {
        return await context.Users.AnyAsync(u => u.Username == username);
    }

    public async Task CreateAccountAsync(CreateAccountDto dto)
    {
        var newUser = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.TempPassword),
            RoleId = dto.RoleId
        };

        context.Users.Add(newUser);
        await context.SaveChangesAsync();
    }
}