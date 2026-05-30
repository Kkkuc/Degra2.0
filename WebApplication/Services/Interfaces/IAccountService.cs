using System.Security.Claims;
using WebApplication.DTOs.Account;

namespace WebApplication.Services.Interfaces;

public interface IAccountService
{
    Task<ClaimsPrincipal?> AuthenticateUserAsync(LoginDto dto);
}