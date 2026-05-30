using WebApplication.DTOs.Admin;

namespace WebApplication.Services.Interfaces;

public interface IAdminService
{
    Task<IEnumerable<UserListDto>> GetUsersForIndexAsync();
    Task<Dictionary<int, string>> GetRolesDropdownListAsync();
    Task<bool> UserExistsAsync(string username);
    Task CreateAccountAsync(CreateAccountDto dto);
}