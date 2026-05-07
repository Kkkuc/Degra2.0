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

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        //do popatrzenia i ulepszenia
        var user = _context.Users.Include(u => u.Role).FirstOrDefault(u => u.Username == username && u.PasswordHash == password);

        if (user != null)
        {
            // 2. Tworzenie Claims (Oświadczeń o użytkowniku)
            var claims = new List<Claim>
            {
             // Jeśli dodawałeś ID, musisz dodać .ToString()! Liczba (int) wyrzuci błąd BinaryReader.
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
    
                // Upewnij się, że dodajesz .ToString(), chyba że Username to na 100% typ string.
                new Claim(ClaimTypes.Name, user.Username), 
    
                new Claim(ClaimTypes.Role, user.Role.Name.ToString())
            };

            // 3. Utworzenie ClaimsIdentity (Twoje główne zadanie!)
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // 4. Konfiguracja właściwości logowania
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true // Ciasteczko przetrwa zamknięcie przeglądarki
            };

            // 5. Zalogowanie użytkownika
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
}