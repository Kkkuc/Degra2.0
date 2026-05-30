using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication.DTOs.Scheduler;
using WebApplication.Services.Interfaces;
using SchedulerViewModel = WebApplication.Models.SchedulerViewModel;

namespace WebApplication.Controllers;

public class SchedulerController(ISchedulerService schedulerService) : Controller
{
    public async Task<IActionResult> Index(
        int? fieldId, int? semId,
        string gW, string gC, string gPs, string gP, string gL, string gJ, string gS, string gWf)
    {
        // Pakujemy parametry wejściowe z adresu URL w czytelny obiekt DTO
        var filter = new SchedulerFilterDto(fieldId, semId, gW, gC, gL, gPs, gP, gS);

        try
        {
            // Pobieramy dane słownikowe do filtrów na górze strony
            var fields = await schedulerService.GetFieldsOfStudyDropdownAsync();
            var semesters = await schedulerService.GetSemestersDropdownAsync();

            ViewBag.Fields = new SelectList(fields, "Key", "Value", fieldId);
            ViewBag.Semesters = new SelectList(semesters, "Key", "Value", semId);

            // Pobieramy w pełni przetworzony model planu zajęć
            var viewModel = await schedulerService.GetSchedulerDataAsync(filter);
            return View(viewModel);
        }
        catch
        {
            // Bezpieczny fallback w razie bazy danych offline lub pustego zestawu danych
            ViewBag.Fields = new SelectList(new Dictionary<int, string>());
            ViewBag.Semesters = new SelectList(new Dictionary<int, string>());

            return View(new SchedulerViewModel());
        }
    }
}