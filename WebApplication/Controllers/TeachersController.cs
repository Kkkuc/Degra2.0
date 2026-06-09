using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers;

public class TeachersController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}