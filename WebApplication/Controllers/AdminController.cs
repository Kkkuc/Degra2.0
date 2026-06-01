using Microsoft.AspNetCore.Mvc;
using WebApplication.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication.Models;
[Authorize(Roles = "Moderator")]
public class AdminController : Controller
{

    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _context.Users.Include(u => u.Role).ToListAsync();
        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> UtworzKonto()
    {
        var roles = await _context.Roles.ToListAsync();
        ViewBag.Roles = new SelectList(roles, "Id", "Name");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UtworzKonto(string username, string email, string tempPassword, int roleId)
    {
        if (await _context.Users.AnyAsync(u => u.Username == username))
        {
            ModelState.AddModelError(string.Empty, "Użytkownik o takiej nazwie już istnieje.");
            ViewBag.Roles = new SelectList(await _context.Roles.ToListAsync(), "Id", "Name");
            return View();
        }

        var newUser = new User
        {
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword),
            RoleId = roleId
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Konto dla {username} zostało utworzone!";
        return RedirectToAction("Index");
    }
}