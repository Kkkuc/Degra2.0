using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication.DTOs.Admin;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

[Authorize(Roles = "Moderator")]
public class AdminController(IAdminService adminService, ITimetablesService timetablesService) : Controller
{
    private async Task PopulateRolesBagAsync(object? selectedValue = null)
    {
        var roles = await adminService.GetRolesDropdownListAsync();
        ViewBag.Roles = new SelectList(roles, "Key", "Value", selectedValue);
    }
    
    
    public async Task<IActionResult> Index()
    {
        var data = await adminService.GetUsersForIndexAsync();
        await PopulateRolesBagAsync();
        
        ViewBag.SubjectId = new SelectList(await timetablesService.GetAllSubjectsAsync(), "Id", "Name");
        ViewBag.TeacherId = new SelectList(await timetablesService.GetAllTeachersAsync(), "Id", "FirstName");
        ViewBag.RoomId = new SelectList(await timetablesService.GetAllRoomsAsync(), "Id", "RoomNumber");
        ViewBag.GroupId = new SelectList(await timetablesService.GetAllGroupsAsync(), "Id", "Name");
        
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
            
            await PopulateRolesBagAsync(dto.RoleId);
            ViewBag.SubjectId = new SelectList(await timetablesService.GetAllSubjectsAsync(), "Id", "Name");
            ViewBag.TeacherId = new SelectList(await timetablesService.GetAllTeachersAsync(), "Id", "FirstName");
            ViewBag.RoomId = new SelectList(await timetablesService.GetAllRoomsAsync(), "Id", "RoomNumber");
            ViewBag.GroupId = new SelectList(await timetablesService.GetAllGroupsAsync(), "Id", "Name");

            var data = await adminService.GetUsersForIndexAsync();
            return View(nameof(Index), data);
        }

        await adminService.CreateAccountAsync(dto);
        TempData["SuccessMessage"] = $"Konto dla {dto.Username} zostało utworzone!";
        return RedirectToAction(nameof(Index));
    }
}