using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplication.Data;

namespace WebApplication.Controllers
{
    public class TeachersController : Controller
    {
        private readonly AppDbContext _context;

        public TeachersController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Get all teachers from the database, ordered alphabetically by Last Name
            var teachers = await _context.Teachers
                                         .OrderBy(t => t.LastName)
                                         .ToListAsync();
            
            return View(teachers);
        }
    }
}