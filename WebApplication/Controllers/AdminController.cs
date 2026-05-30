using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication.DTOs.Admin;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

[Authorize(Roles = "Moderator")]
public class AdminController(IAdminService adminService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var data = await adminService.GetUsersForIndexAsync();
        return View(data);
    }

    [HttpGet]
    public async Task<IActionResult> CreateAccount()
    {
        var roles = await adminService.GetRolesDropdownListAsync();
        ViewBag.Roles = new SelectList(roles, "Key", "Value");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAccount(CreateAccountDto dto)
    {
        if (!ModelState.IsValid)
        {
            var roles = await adminService.GetRolesDropdownListAsync();
            ViewBag.Roles = new SelectList(roles, "Key", "Value", dto.RoleId);
            return View(dto);
        }

        if (await adminService.UserExistsAsync(dto.Username))
        {
            ModelState.AddModelError(string.Empty, "Użytkownik o takiej nazwie już istnieje.");
            
            var roles = await adminService.GetRolesDropdownListAsync();
            ViewBag.Roles = new SelectList(roles, "Key", "Value", dto.RoleId);
            return View(dto);
        }

        await adminService.CreateAccountAsync(dto);

        TempData["SuccessMessage"] = $"Konto dla {dto.Username} zostało utworzone!";
        return RedirectToAction(nameof(Index));
    }
}