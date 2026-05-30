using Microsoft.AspNetCore.Mvc;
using WebApplication.DTOs.Teacher;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

public class TeachersController(ITeachersService teachersService) : Controller
{
    // GET: Teachers
    public async Task<IActionResult> Index()
    {
        var data = await teachersService.GetAllAsync();
        return View(data);
    }

    // GET: Teachers/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var teacher = await teachersService.GetByIdAsync(id);
        if (teacher == null)
        {
            return NotFound();
        }

        return View(teacher);
    }

    // GET: Teachers/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Teachers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TeacherFormDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        await teachersService.CreateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    // GET: Teachers/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var teacher = await teachersService.GetFormByIdAsync(id);
        if (teacher == null)
        {
            return NotFound();
        }

        return View(teacher);
    }

    // POST: Teachers/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TeacherFormDto dto)
    {
        if (id != dto.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var success = await teachersService.UpdateAsync(dto);
        if (!success)
        {
            if (!await teachersService.ExistsAsync(dto.Id))
            {
                return NotFound();
            }

            ModelState.AddModelError(string.Empty, "Wystąpił nieoczekiwany błąd podczas zapisu zmian.");
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Teachers/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var teacher = await teachersService.GetByIdAsync(id);
        if (teacher == null)
        {
            return NotFound();
        }

        return View(teacher);
    }

    // POST: Teachers/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var success = await teachersService.DeleteAsync(id);
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
}