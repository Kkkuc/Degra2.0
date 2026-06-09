using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication.DTOs.Admin;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

[Authorize(Roles = "Moderator")]
public class AdminController(IAdminService adminService, ITimetablesService timetablesService, IFacultiesService facultiesService) : Controller
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

    public async Task<IActionResult> Buildings()
    {
        return View();
    }

    public async Task<IActionResult> Index()
    {
        return View();
    }

    public async Task<IActionResult> Timetable()
    {
        await LoadViewDataAsync();
        return View();
    }
    
    public async Task<IActionResult> FieldOfStudy()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult Groups()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult Semesters()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult Specializations()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult Students()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult Subjects()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult Teachers()
    {
        return View();
    }
    
    public async Task<IActionResult> Faculties()
    {
        var faculties = await facultiesService.GetAllAsync();
        ViewBag.Faculties = new SelectList(faculties.OrderBy(f => f.Name), "Id", "Name");
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

}
