using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    public class ScraperController : Controller
    {
        private readonly TimetableCrawler _crawler;
        private readonly AppDbContext _context;

        // Inject both the crawler and the database context
        public ScraperController(TimetableCrawler crawler, AppDbContext context)
        {
            _crawler = crawler;
            _context = context;
        }

        // This displays the page with the button
        public async Task<IActionResult> Index()
        {
            // Get some quick stats to prove the database is working
            ViewBag.TeacherCount = await _context.Teachers.CountAsync();
            ViewBag.TimetableCount = await _context.Timetables.CountAsync();
            
            return View();
        }

        // This runs when you click the "Start Scraping" button
        [HttpPost]
        public async Task<IActionResult> RunScraper()
        {
            // Execute the scraper
            await _crawler.CrawlAndSaveAsync();
            
            // Send a success message back to the view
            TempData["SuccessMessage"] = "Scraping finished successfully! Database is updated.";
            
            return RedirectToAction("Index");
        }
    }
}