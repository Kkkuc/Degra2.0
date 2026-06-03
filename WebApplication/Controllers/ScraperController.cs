using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

public class ScraperController : Controller
{
    private readonly IScraperService _scraperService;
    private readonly AppDbContext _context;

    public ScraperController(IScraperService scraperService, AppDbContext context)
    {
        _scraperService = scraperService;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.TeacherCount = await _context.Teachers.CountAsync();
        ViewBag.SubjectCount = await _context.Subjects.CountAsync();
        ViewBag.TimetableCount = await _context.Timetables.CountAsync();
        ViewBag.LastSynced = null;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RunScraper()
    {
        try
        {
            string url = "https://degra.wi.pb.edu.pl/rozklady/webservices.php";
            await _scraperService.ScrapeAndSaveAsync(url);
            TempData["SuccessMessage"] = "Data successfully synchronized!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Synchronization failed: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
}