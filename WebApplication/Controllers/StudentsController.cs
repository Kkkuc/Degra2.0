using Microsoft.AspNetCore.Mvc;
using WebApplication.DTOs.Student;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

public class StudentsController(IStudentsService studentsService) : Controller
{
    // GET: Students
    public async Task<IActionResult> Index()
    {
        var data = await studentsService.GetAllAsync();
        return View(data);
    }

    // GET: Students/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var student = await studentsService.GetDetailsByIdAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    // GET: Students/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Students/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StudentFormDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        await studentsService.CreateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    // GET: Students/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var student = await studentsService.GetFormByIdAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    // POST: Students/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, StudentFormDto dto)
    {
        if (id != dto.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var success = await studentsService.UpdateAsync(dto);
        if (success)
        {
            return RedirectToAction(nameof(Index));
        }

        if (!await studentsService.ExistsAsync(dto.Id))
        {
            return NotFound();
        }

        ModelState.AddModelError(string.Empty, "Wystąpił nieoczekiwany błąd podczas zapisu zmian.");
        return View(dto);
    }

    // GET: Students/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var student = await studentsService.GetByIdAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    // POST: Students/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var success = await studentsService.DeleteAsync(id);
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
}