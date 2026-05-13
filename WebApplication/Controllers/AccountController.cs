using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication.Data;
using Microsoft.EntityFrameworkCore;

public class AccountController : Controller
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        //do popatrzenia i ulepszenia
        var user = _context.Users.Include(u => u.Role).FirstOrDefault(u => u.Username == username && u.PasswordHash == password);

        if (user != null)
        {
        
            var claims = new List<Claim>
            {      
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
    
                new Claim(ClaimTypes.Name, user.Username), 
    
                new Claim(ClaimTypes.Role, user.Role.Name.ToString())
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

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Nieprawidłowy login lub hasło");
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}