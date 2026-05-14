using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers;

public class TeachersController : Controller
{
    private readonly AppDbContext _context;

    public TeachersController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var teachers = await _context.Teachers.ToListAsync();
            return View(teachers);
        }
        catch
        {
            return View(new List<Teacher>());
        }
    }
}