using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers;

public class SemestersController(ISemestersService semestersService) : Controller
{
    // GET: Semesters
    public async Task<IActionResult> Index()
    {
        var data = await semestersService.GetAllWithAcademicYearAsync();
        return View(data);
    }

    // GET: Semesters/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var semester = await semestersService.GetByIdWithAcademicYearAsync(id.Value);
        if (semester == null)
        {
            return NotFound();
        }

        return View(semester);
    }

    // GET: Semesters/Create
    public async Task<IActionResult> Create()
    {
        var academicYears = await semestersService.GetAllAcademicYearsAsync();
        ViewData["AcademicYearId"] = new SelectList(academicYears, "Id", "Name");
        return View();
    }

    // POST: Semesters/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,AcademicYearId,Name,StartDate,EndDate")] Semester semester)
    {
        if (ModelState.IsValid)
        {
            await semestersService.CreateAsync(semester);
            return RedirectToAction(nameof(Index));
        }

        var academicYears = await semestersService.GetAllAcademicYearsAsync();
        ViewData["AcademicYearId"] = new SelectList(academicYears, "Id", "Name", semester.AcademicYearId);
        return View(semester);
    }

    // GET: Semesters/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var semester = await semestersService.GetByIdAsync(id.Value);
        if (semester == null)
        {
            return NotFound();
        }

        var academicYears = await semestersService.GetAllAcademicYearsAsync();
        ViewData["AcademicYearId"] = new SelectList(academicYears, "Id", "Name", semester.AcademicYearId);
        return View(semester);
    }

    // POST: Semesters/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,AcademicYearId,Name,StartDate,EndDate")] Semester semester)
    {
        if (id != semester.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await semestersService.UpdateAsync(semester);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await semestersService.ExistsAsync(semester.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        var academicYears = await semestersService.GetAllAcademicYearsAsync();
        ViewData["AcademicYearId"] = new SelectList(academicYears, "Id", "Name", semester.AcademicYearId);
        return View(semester);
    }

    // GET: Semesters/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var semester = await semestersService.GetByIdWithAcademicYearAsync(id.Value);
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
        await semestersService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}