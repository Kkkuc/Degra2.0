using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.DTOs.Group;
using WebApplication.Models;
using WebApplication.Models.enums;
using WebApplication.Services;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

public class GroupsController(IGroupsService groupsService) : Controller
{
    // GET: Groups
    public async Task<IActionResult> Index()
    {
        var data = await groupsService.GetAllForIndexAsync();
        return View(data);
    }

    // GET: Groups/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var group = await groupsService.GetDetailsByIdAsync(id);
        if (group == null)
        {
            return NotFound();
        }

        return View(group); 
    }
    
    // GET: Groups/Create
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View();
    }

    // POST: Groups/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GroupFormDto dto)
    {
        if (ModelState.IsValid)
        {
            await groupsService.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        await PopulateDropdownsAsync(dto);
        return View(dto);
    }

    // GET: Groups/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var groupForm = await groupsService.GetFormByIdAsync(id);
        if (groupForm == null)
        {
            return NotFound();
        }

        await PopulateDropdownsAsync(groupForm);
        return View(groupForm); 
    }

    // POST: Groups/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, GroupFormDto dto)
    {
        if (id != dto.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var success = await groupsService.UpdateAsync(dto);
            if (!success)
            {
                if (!await groupsService.ExistsAsync(dto.Id))
                {
                    return NotFound();
                }
                ModelState.AddModelError(string.Empty, "Wystąpił błąd podczas aktualizacji grupy.");
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        await PopulateDropdownsAsync(dto);
        return View(dto);
    }

    // GET: Groups/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var group = await groupsService.GetDetailsByIdAsync(id);
        if (group == null)
        {
            return NotFound();
        }

        return View(group); 
    }

    // POST: Groups/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var success = await groupsService.DeleteAsync(id);
        if (!success)
        {
            return NotFound();
        }
        
        return RedirectToAction(nameof(Index));
    }
    
    private async Task PopulateDropdownsAsync(GroupFormDto? dto = null)
    {
        var fieldsOfStudies = await groupsService.GetFieldsOfStudyDropdownListAsync();
        var semesters = await groupsService.GetSemestersDropdownListAsync();
        var specializations = await groupsService.GetSpecializationsDropdownListAsync();

        ViewData["FieldOfStudyId"] = new SelectList(fieldsOfStudies, "Key", "Value", dto?.FieldOfStudyId);
        ViewData["SemesterId"] = new SelectList(semesters, "Key", "Value", dto?.SemesterId);
        ViewData["SpecializationId"] = new SelectList(specializations, "Key", "Value", dto?.SpecializationId);
        
        // Dla enuma przekazujemy wybraną wartość, jeśli obiekt dto istnieje
        ViewData["ClassType"] = dto != null 
            ? new SelectList(Enum.GetValues<ClassType>(), dto.ClassType)
            : new SelectList(Enum.GetValues<ClassType>());
    }
}