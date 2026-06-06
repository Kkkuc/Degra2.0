using Microsoft.AspNetCore.Mvc;
using WebApplication.DTOs.Subject;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

public class SubjectsController(ISubjectsService subjectsService) : Controller
{
    // GET: Subjects
    public async Task<IActionResult> Index()
    {
        var data = await subjectsService.GetAllForIndexAsync();
        return View(data);
    }

    // GET: Subjects/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var subject = await subjectsService.GetDetailsByIdAsync(id);
        if (subject == null)
        {
            return NotFound();
        }

        return View(subject);
    }

    // GET: Subjects/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Subjects/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SubjectFormDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        await subjectsService.CreateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    // GET: Subjects/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var subject = await subjectsService.GetFormByIdAsync(id);
        if (subject == null)
        {
            return NotFound();
        }

        return View(subject);
    }

    // POST: Subjects/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SubjectFormDto dto)
    {
        if (id != dto.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var success = await subjectsService.UpdateAsync(dto);
        if (success)
        {
            return RedirectToAction(nameof(Index));
        }

        if (!await subjectsService.ExistsAsync(dto.Id))
        {
            return NotFound();
        }

        ModelState.AddModelError(string.Empty, "Wystąpił nieoczekiwany błąd podczas zapisu zmian.");
        return View(dto);
    }

    // GET: Subjects/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var subject = await subjectsService.GetDetailsByIdAsync(id);
        if (subject == null)
        {
            return NotFound();
        }

        return View(subject);
    }

    // POST: Subjects/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var success = await subjectsService.DeleteAsync(id);
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
}