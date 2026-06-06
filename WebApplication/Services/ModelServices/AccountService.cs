using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.DTOs.Account;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services.ModelServices;

public class AccountService(AppDbContext context) : IAccountService
{
    public async Task<ClaimsPrincipal?> AuthenticateUserAsync(LoginDto dto)
    {
        var userData = await context.Users
            .Where(u => u.Username == dto.Username)
            .Select(u => new
            {
                u.Id,
                u.Username,
                u.PasswordHash,
                RoleName = u.Role != null ? u.Role.Name : string.Empty
            })
            .FirstOrDefaultAsync();

        if (userData == null)
        {
            return null;
        }
        
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, userData.PasswordHash))
        {
            return null;
        }
        
        // Tymczasowe proste porównanie (usuń po wdrożeniu hashowania!)
        // if (user.PasswordHash != dto.Password) 
        // {
        //     return null;
        // }

        var claims = new List<Claim>
        {      
            new Claim(ClaimTypes.NameIdentifier, userData.Id.ToString()), 
            new Claim(ClaimTypes.Name, userData.Username), 
            new Claim(ClaimTypes.Role, userData.RoleName)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        
        return new ClaimsPrincipal(claimsIdentity);
    }
}