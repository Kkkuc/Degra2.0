using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;

namespace WebApplication.Controllers;

public class RoomsController : Controller
{
    private readonly AppDbContext _context;

    public RoomsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var rooms = await _context.Rooms
            .Include(r => r.Building)
            .ToListAsync();
        return View(rooms);
    }
}