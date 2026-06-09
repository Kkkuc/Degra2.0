using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers;

public class RoomsController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}