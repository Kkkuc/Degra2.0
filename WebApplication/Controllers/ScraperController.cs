using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

[Authorize(Roles = "Moderator")]
public class ScraperController(IScraperService scraperService, AppDbContext context) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.TeacherCount = await context.Teachers.CountAsync();
        ViewBag.SubjectCount = await context.Subjects.CountAsync();
        ViewBag.TimetableCount = await context.Timetables.CountAsync();
        ViewBag.LastSynced = null;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RunScraper()
    {
        try
        {
            string url = "https://degra.wi.pb.edu.pl/rozklady/webservices.php";
            await scraperService.ScrapeAndSaveAsync(url);
            TempData["SuccessMessage"] = "Data successfully synchronized!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Synchronization failed: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
    
    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile xmlFile)
    {
        if (xmlFile == null || xmlFile.Length == 0)
        {
            TempData["ErrorMessage"] = "Wybrany plik jest nieprawidłowy lub pusty.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            using var stream = xmlFile.OpenReadStream();
            await _scraperService.ImportFromFileAsync(stream);
            TempData["SuccessMessage"] = "Dane z pliku XML zostały pomyślnie zaimportowane!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Błąd podczas importu pliku: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
}