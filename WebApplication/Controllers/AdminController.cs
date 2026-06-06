using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication.DTOs.Admin;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

[Authorize(Roles = "Moderator")]
public class AdminController(IAdminService adminService, ITimetablesService timetablesService) : Controller
{
    private async Task LoadViewDataAsync(object? selectedRoleId = null)
    {
        var roles = await adminService.GetRolesDropdownListAsync();
        ViewBag.Roles = new SelectList(roles, "Key", "Value", selectedRoleId);
        
        ViewBag.SubjectId = new SelectList(await timetablesService.GetAllSubjectsAsync(), "Id", "Name");
        ViewBag.TeacherId = new SelectList(await timetablesService.GetAllTeachersAsync(), "Id", "FirstName");
        ViewBag.RoomId = new SelectList(await timetablesService.GetAllRoomsAsync(), "Id", "RoomNumber");
        ViewBag.GroupId = new SelectList(await timetablesService.GetAllGroupsAsync(), "Id", "Name");
    }
    
    
    public async Task<IActionResult> Index()
    {
        var data = await adminService.GetUsersForIndexAsync();
        await LoadViewDataAsync();
        return View(data);
    }

    [HttpGet]
    public IActionResult CreateAccount() => RedirectToAction(nameof(Index));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAccount(CreateAccountDto dto)
    {
        if (!ModelState.IsValid || await adminService.UserExistsAsync(dto.Username))
        {
            if (await adminService.UserExistsAsync(dto.Username))
            {
                ModelState.AddModelError(string.Empty, "Użytkownik o takiej nazwie już istnieje.");
            }
            
            // Dzięki wspólnej metodzie, nie musisz kopiować ViewBag-ów
            await LoadViewDataAsync(dto.RoleId);
            
            var data = await adminService.GetUsersForIndexAsync();
            return View(nameof(Index), data);
        }

        await adminService.CreateAccountAsync(dto);
        TempData["SuccessMessage"] = $"Konto dla {dto.Username} zostało utworzone!";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GenerateMonthlyStats(int rok, int miesiac)
    {
        try
        {
            var pdfBytes = await adminService.GenerateMonthlyStatsPdfAsync(rok, miesiac);
            return File(pdfBytes, "application/pdf", $"Statystyki_Miesieczne_{rok}_{miesiac:D2}.pdf");
        }
        catch (Exception)
        {
            // Obsługa błędów, np. logowanie i powrót do widoku z komunikatem
            TempData["ErrorMessage"] = "Wystąpił błąd podczas generowania raportu.";
            return RedirectToAction(nameof(Index));
        }
    }
}