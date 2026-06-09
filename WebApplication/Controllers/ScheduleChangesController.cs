using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication.DTOs.ScheduleChange;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

[Authorize(Roles = "Moderator")]
public class ScheduleChangesController(IScheduleChangesService scheduleService) : Controller
{
    // GET: ScheduleChanges
    public async Task<IActionResult> Index()
    {
        var data = await scheduleService.GetAllForIndexAsync();
        return View(data);
    }

    // GET: ScheduleChanges/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var scheduleChange = await scheduleService.GetDetailsByIdAsync(id);
        if (scheduleChange == null)
        {
            return NotFound();
        }

        return View(scheduleChange);
    }

    // GET: ScheduleChanges/Create
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View();
    }

    // POST: ScheduleChanges/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ScheduleChangeFormDto dto)
    {
        if (ModelState.IsValid)
        {
            await scheduleService.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        await PopulateDropdownsAsync(dto);
        return View(dto);
    }

    // GET: ScheduleChanges/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var scheduleChange = await scheduleService.GetFormByIdAsync(id);
        if (scheduleChange == null) return NotFound();

        await PopulateDropdownsAsync(scheduleChange);
        return View(scheduleChange);
    }

    // POST: ScheduleChanges/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ScheduleChangeFormDto dto)
    {
        if (id != dto.Id) return NotFound();

        if (ModelState.IsValid)
        {
            var success = await scheduleService.UpdateAsync(dto);
            if (!success)
            {
                if (!await scheduleService.ExistsAsync(dto.Id)) return NotFound();
                ModelState.AddModelError(string.Empty, "Wystąpił błąd zapisu zmian planu.");
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        await PopulateDropdownsAsync(dto);
        return View(dto);
    }

    // GET: ScheduleChanges/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var scheduleChange = await scheduleService.GetDetailsByIdAsync(id);
        if (scheduleChange == null) return NotFound();

        return View(scheduleChange);
    }

    // POST: ScheduleChanges/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var success = await scheduleService.DeleteAsync(id);
        if (!success) return NotFound();

        return RedirectToAction(nameof(Index));
    }
    
    private async Task PopulateDropdownsAsync(ScheduleChangeFormDto? dto = null)
    {
        var rooms = await scheduleService.GetRoomsDropdownAsync();
        var teachers = await scheduleService.GetTeachersDropdownAsync();
        var timetables = await scheduleService.GetTimetablesDropdownAsync();

        ViewData["NewRoomId"] = new SelectList(rooms, "Key", "Value", dto?.NewRoomId);
        ViewData["NewTeacherId"] = new SelectList(teachers, "Key", "Value", dto?.NewTeacherId);
        ViewData["TimetableId"] = new SelectList(timetables, "Key", "Value", dto?.TimetableId);
    }
}