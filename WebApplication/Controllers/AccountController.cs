using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using WebApplication.DTOs.Account;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

public class AccountController(IAccountService accountService) : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var principal = await accountService.AuthenticateUserAsync(dto);

        if (principal != null)
        {
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true // Zostaje po zamknięciu przeglądarki
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties);

            return RedirectToAction("Index", "Scheduler");
        }

        ModelState.AddModelError(string.Empty, "Niepoprawna nazwa użytkownika lub hasło.");
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
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