using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        var data = await groupsService.GetAllWithRelationsAsync();
        return View(data);
    }

    // GET: Groups/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var group = await groupsService.GetByIdWithRelationsAsync(id.Value);
        if (group == null)
        {
            return NotFound();
        }

        return View(group);
    }

    // GET: Groups/Create
    public async Task<IActionResult> Create()
    {
        var fieldsOfStudies = await groupsService.GetAllFieldsOfStudyAsync();
        var semesters = await groupsService.GetAllSemestersAsync();
        var specializations = await groupsService.GetAllSpecializationsAsync();
        ViewData["FieldOfStudyId"] = new SelectList(fieldsOfStudies, "Id", "Name");
        ViewData["SemesterId"] = new SelectList(semesters, "Id", "Name");
        ViewData["SpecializationId"] = new SelectList(specializations, "Id", "Name");
        ViewData["ClassType"] = new SelectList(Enum.GetValues<ClassType>());
        return View();
    }

    // POST: Groups/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Id,SemesterId,FieldOfStudyId,SpecializationId,ClassType,Name")]
        Group @group)
    {
        if (ModelState.IsValid)
        {
            await groupsService.CreateAsync(group);
            return RedirectToAction(nameof(Index));
        }

        var fieldsOfStudies = await groupsService.GetAllFieldsOfStudyAsync();
        var semesters = await groupsService.GetAllSemestersAsync();
        var specializations = await groupsService.GetAllSpecializationsAsync();
        ViewData["FieldOfStudyId"] = new SelectList(fieldsOfStudies, "Id", "Name", group.FieldOfStudyId);
        ViewData["SemesterId"] = new SelectList(semesters, "Id", "Name", group.SemesterId);
        ViewData["SpecializationId"] = new SelectList(specializations, "Id", "Name", group.SpecializationId);
        ViewData["ClassType"] = new SelectList(Enum.GetValues<ClassType>());
        return View(group);
    }

    // GET: Groups/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var group = await groupsService.GetByIdAsync(id.Value);
        if (group == null)
        {
            return NotFound();
        }

        var fieldsOfStudies = await groupsService.GetAllFieldsOfStudyAsync();
        var semesters = await groupsService.GetAllSemestersAsync();
        var specializations = await groupsService.GetAllSpecializationsAsync();
        ViewData["FieldOfStudyId"] = new SelectList(fieldsOfStudies, "Id", "Name", group.FieldOfStudyId);
        ViewData["SemesterId"] = new SelectList(semesters, "Id", "Name", group.SemesterId);
        ViewData["SpecializationId"] = new SelectList(specializations, "Id", "Name", group.SpecializationId);
        ViewData["ClassType"] = new SelectList(Enum.GetValues<ClassType>());
        return View(group);
    }

    // POST: Groups/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("Id,SemesterId,FieldOfStudyId,SpecializationId,ClassType,Name")]
        Group @group)
    {
        if (id != group.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await groupsService.UpdateAsync(group);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await groupsService.ExistsAsync(group.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        var fieldsOfStudies = await groupsService.GetAllFieldsOfStudyAsync();
        var semesters = await groupsService.GetAllSemestersAsync();
        var specializations = await groupsService.GetAllSpecializationsAsync();
        ViewData["FieldOfStudyId"] = new SelectList(fieldsOfStudies, "Id", "Name", group.FieldOfStudyId);
        ViewData["SemesterId"] = new SelectList(semesters, "Id", "Name", group.SemesterId);
        ViewData["SpecializationId"] = new SelectList(specializations, "Id", "Name", group.SpecializationId);
        ViewData["ClassType"] = new SelectList(Enum.GetValues<ClassType>());
        return View(group);
    }

    // GET: Groups/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var group = await groupsService.GetByIdWithRelationsAsync(id.Value);
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
        await groupsService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}