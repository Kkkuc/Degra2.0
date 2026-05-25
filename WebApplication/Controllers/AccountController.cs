using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;

namespace WebApplication.Controllers;

public class AccountController(AppDbContext context) : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        //do popatrzenia i ulepszenia
        var user = await context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == username);
        //co za gorwno
        if (user != null /*&& BCrypt.Net.BCrypt.Verify(password, user.PasswordHash) jak dodamy hashwowanie*/)
        {

            var claims = new List<Claim>
            {      
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
    
                new Claim(ClaimTypes.Name, user.Username), 
    
                new Claim(ClaimTypes.Role, user.Role!.Name)
            };


            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true //pytanie zcy cchcemy zeby zostawały po zamknieciu
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Scheduler");
        }

        ModelState.AddModelError("", "Invalid username or password.");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Scheduler");
    }

    [HttpGet]
    public IActionResult Settings()
    {
        return View();
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}