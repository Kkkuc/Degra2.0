using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication.DTOs.Semester;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

public class SemestersController(ISemestersService semestersService) : Controller
{
    // GET: Semesters
    public async Task<IActionResult> Index()
    {
        var data = await semestersService.GetAllForIndexAsync();
        return View(data);
    }

    // GET: Semesters/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var semester = await semestersService.GetDetailsByIdAsync(id);
        if (semester == null)
        {
            return NotFound();
        }

        return View(semester);
    }

    // GET: Semesters/Create
    public async Task<IActionResult> Create()
    {
        await PopulateAcademicYearsDropdownAsync();
        return View();
    }

    // POST: Semesters/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SemesterFormDto dto)
    {
        if (ModelState.IsValid)
        {
            await semestersService.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        await PopulateAcademicYearsDropdownAsync(dto.AcademicYearId);
        return View(dto);
    }

    // GET: Semesters/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var semester = await semestersService.GetFormByIdAsync(id);
        if (semester == null)
        {
            return NotFound();
        }

        await PopulateAcademicYearsDropdownAsync(semester.AcademicYearId);
        return View(semester);
    }

    // POST: Semesters/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SemesterFormDto dto)
    {
        if (id != dto.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var success = await semestersService.UpdateAsync(dto);
            if (!success)
            {
                if (!await semestersService.ExistsAsync(dto.Id))
                {
                    return NotFound();
                }
                ModelState.AddModelError(string.Empty, "Wystąpił nieoczekiwany błąd podczas zapisu.");
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        await PopulateAcademicYearsDropdownAsync(dto.AcademicYearId);
        return View(dto);
    }

    // GET: Semesters/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var semester = await semestersService.GetDetailsByIdAsync(id);
        if (semester == null)
        {
            return NotFound();
        }

        return View(semester);
    }

    // POST: Semesters/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var success = await semestersService.DeleteAsync(id);
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
    
    private async Task PopulateAcademicYearsDropdownAsync(int? selectedId = null)
    {
        var academicYears = await semestersService.GetAcademicYearsDropdownAsync();
        ViewData["AcademicYearId"] = new SelectList(academicYears, "Key", "Value", selectedId);
    }
}