using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication.DTOs.FieldOfStudy;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

public class FieldsOfStudiesController(IFieldsOfStudiesService fieldsService) : Controller
{
    // GET: FieldsOfStudies
    public async Task<IActionResult> Index()
    {
        var data = await fieldsService.GetAllForIndexAsync();
        return View(data);
    }

    // GET: FieldsOfStudies/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var dto = await fieldsService.GetDetailsByIdAsync(id);
        if (dto == null)
        {
            return NotFound();
        }

        return View(dto);
    }

    // GET: FieldsOfStudies/Create
    public async Task<IActionResult> Create()
    {
        var faculties = await fieldsService.GetFacultyDropdownListAsync();
        ViewData["FacultyId"] = new SelectList(faculties, "Key", "Value");
        return View();
    }

    // POST: FieldsOfStudies/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FieldOfStudyFormDto dto)
    {
        if (!ModelState.IsValid)
        {
            var faculties = await fieldsService.GetFacultyDropdownListAsync();
            ViewData["FacultyId"] = new SelectList(faculties, "Key", "Value", dto.FacultyId);
            return View(dto);
        }

        await fieldsService.CreateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    // GET: FieldsOfStudies/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await fieldsService.GetFormByIdAsync(id);
        if (dto == null)
        {
            return NotFound();
        }

        var faculties = await fieldsService.GetFacultyDropdownListAsync();
        ViewData["FacultyId"] = new SelectList(faculties, "Key", "Value", dto.FacultyId);
        return View(dto);
    }

    // POST: FieldsOfStudies/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, FieldOfStudyFormDto dto)
    {
        if (id != dto.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            var faculties = await fieldsService.GetFacultyDropdownListAsync();
            ViewData["FacultyId"] = new SelectList(faculties, "Key", "Value", dto.FacultyId);
            return View(dto);
        }

        var updated = await fieldsService.UpdateAsync(dto);
        if (!updated)
        {
            if (!await fieldsService.ExistsAsync(dto.Id))
            {
                return NotFound();
            }

            ModelState.AddModelError(string.Empty, "Wystąpił błąd podczas zapisu.");
            var faculties = await fieldsService.GetFacultyDropdownListAsync();
            ViewData["FacultyId"] = new SelectList(faculties, "Key", "Value", dto.FacultyId);
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: FieldsOfStudies/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await fieldsService.GetDetailsByIdAsync(id);
        if (dto == null)
        {
            return NotFound();
        }

        return View(dto);
    }

    // POST: FieldsOfStudies/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await fieldsService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}