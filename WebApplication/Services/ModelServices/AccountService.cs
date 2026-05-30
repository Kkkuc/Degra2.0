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
        var user = await context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == dto.Username);

        if (user == null)
        {
            return null;
        }
        
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
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
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
            new Claim(ClaimTypes.Name, user.Username), 
            new Claim(ClaimTypes.Role, user.Role!.Name)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        
        return new ClaimsPrincipal(claimsIdentity);
    }
}