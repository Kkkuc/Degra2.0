using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

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
        try
        {
            var rooms = await _context.Rooms
                .Include(r => r.Building)
                .ToListAsync();
            return View(rooms);
        }
        catch
        {
            return View(new List<Room>());
        }
    }
}