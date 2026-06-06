using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    public IActionResult TajnyPanel()
    {
        return View(); // Zmienione z Content na View
    }

    [Authorize(Roles = "Moderator")]
    public IActionResult PanelModeratora()
    {
        return View(); // Zmienione z Content na View
    }
   
}