using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers;

[Authorize(Roles = "Moderator")]
public class AdminController(AppDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var users = await context.Users.Include(u => u.Role).ToListAsync();
        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> CreateAccount()
    {
        var roles = await context.Roles.ToListAsync();
        ViewBag.Roles = new SelectList(roles, "Id", "Name");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount(string username, string email, string tempPassword, int roleId)
    {
        if (await context.Users.AnyAsync(u => u.Username == username))
        {
            ModelState.AddModelError(string.Empty, "Użytkownik o takiej nazwie już istnieje.");
            ViewBag.Roles = new SelectList(await context.Roles.ToListAsync(), "Id", "Name");
            return View();
        }

        var newUser = new User
        {
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword),
            RoleId = roleId
        };

        context.Users.Add(newUser);
        await context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Konto dla {username} zostało utworzone!";
        return RedirectToAction("Index");
    }
}