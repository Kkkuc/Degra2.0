using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication.DTOs.Admin;
using WebApplication.DTOs.Building;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

[Authorize(Roles = "Moderator")]
public class AdminController(IAdminService adminService, ITimetablesService timetablesService, IBuildingsService buildingsService) : Controller
{
    private async Task LoadViewDataAsync()
    {
        var subjects = await timetablesService.GetAllSubjectsAsync();
        ViewBag.SubjectId = new SelectList(subjects.OrderBy(s => s.Name), "Id", "Name");

        var teachers = await timetablesService.GetAllTeachersAsync();
        ViewBag.TeacherId =
            new SelectList(teachers.OrderBy(t => t.LastName).ThenBy(t => t.FirstName).ThenBy(t => t.AcademicTitle),
                "Id", "FullDisplayName");

        var rooms = await timetablesService.GetAllRoomsAsync();
        ViewBag.RoomId = new SelectList(rooms.OrderBy(r => r.RoomNumber), "Id", "RoomNumber");

        var groups = await timetablesService.GetAllGroupsAsync();
        ViewBag.GroupId = new SelectList(groups.OrderBy(g => g.Name), "Id", "Name");
    }

    private async Task LoadRolesAsync(object? selectedRoleId = null)
    {
        var roles = await adminService.GetRolesDropdownListAsync();
        ViewBag.Roles = new SelectList(roles, "Key", "Value", selectedRoleId);
    }

    private async Task LoadBuildingDataAsync(object? selectedFacultyId = null)
    {
        var faculties = await buildingsService.GetFacultyDropdownListAsync();
        ViewBag.Faculties = new SelectList(faculties, "Key", "Value", selectedFacultyId);
    }

    public async Task<IActionResult> Buildings()
    {
        await LoadBuildingDataAsync();
        return View();
    }

    public async Task<IActionResult> Index()
    {
        await LoadViewDataAsync();
        return View();
    }

    public async Task<IActionResult> Timetable()
    {
        await LoadViewDataAsync();
        return View();
    }

    public IActionResult Reports() => View();

    public async Task<IActionResult> Users(object? selectedRoleId = null)
    {
        await LoadRolesAsync(selectedRoleId);
        
        var data = await adminService.GetUsersForIndexAsync();
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
            
            await LoadViewDataAsync();
            await LoadRolesAsync(dto.RoleId);

            var data = await adminService.GetUsersForIndexAsync();
            return View(nameof(Users), data);
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

    [HttpGet]
    public async Task<IActionResult> BuildingsData(string? search, int? facultyId)
    {
        var buildings = await buildingsService.GetAllForAdminAsync(search, facultyId);
        return Json(buildings);
    }

    [HttpGet]
    public async Task<IActionResult> BuildingData(int id)
    {
        var building = await buildingsService.GetFormByIdAsync(id);
        if (building == null)
        {
            return NotFound();
        }

        return Json(building);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBuilding([FromBody] BuildingFormDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await buildingsService.CreateAsync(dto);
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateBuilding(int id, [FromBody] BuildingFormDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var success = await buildingsService.UpdateAsync(dto);
        return success ? Ok() : NotFound();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteBuilding(int id)
    {
        await buildingsService.DeleteAsync(id);
        return Ok();
    }

    
}
