using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers;

public class ScheduleChangesController(IScheduleChangesService scheduleService) : Controller
{
    // GET: ScheduleChanges
    public async Task<IActionResult> Index()
    {
        var data = await scheduleService.GetAllWithRelationsAsync();
        return View(data);
    }

    // GET: ScheduleChanges/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var scheduleChange = await scheduleService.GetByIdWithRelationsAsync(id.Value);
        if (scheduleChange == null)
        {
            return NotFound();
        }

        return View(scheduleChange);
    }

    // GET: ScheduleChanges/Create
    public async Task<IActionResult> Create()
    {
        var allNewRooms = await scheduleService.GetAllRoomsAsync();
        var allNewTeachers = await scheduleService.GetTeachersLookupAsync();
        var allTimetables = await scheduleService.GetTimetablesLookupAsync();
        
        ViewData["NewRoomId"] = new SelectList(allNewRooms, "Id", "RoomNumber");
        ViewData["NewTeacherId"] = new SelectList(allNewTeachers, "Id", "FullName");
        ViewData["TimetableId"] = new SelectList(allTimetables, "Id", "Text");
        return View();
    }

    // POST: ScheduleChanges/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Id,TimetableId,ChangeDate,NewRoomId,NewTeacherId,NewStartTime,NewEndTime")]
        ScheduleChange scheduleChange)
    {
        if (ModelState.IsValid)
        {
            await scheduleService.CreateAsync(scheduleChange);
            return RedirectToAction(nameof(Index));
        }

        var allNewRooms = await scheduleService.GetAllRoomsAsync();
        var allNewTeachers = await scheduleService.GetTeachersLookupAsync();
        var allTimetables = await scheduleService.GetTimetablesLookupAsync();

        ViewData["NewRoomId"] = new SelectList(allNewRooms, "Id", "RoomNumber", scheduleChange.NewRoomId);
        ViewData["NewTeacherId"] = new SelectList(allNewTeachers, "Id", "FullName", scheduleChange.NewTeacherId);
        ViewData["TimetableId"] = new SelectList(allTimetables, "Id", "Text", scheduleChange.TimetableId);
        return View(scheduleChange);
    }

    // GET: ScheduleChanges/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var scheduleChange = await scheduleService.GetByIdAsync(id.Value);
        if (scheduleChange == null)
        {
            return NotFound();
        }

        var allNewRooms = await scheduleService.GetAllRoomsAsync();
        var allNewTeachers = await scheduleService.GetTeachersLookupAsync();
        var allTimetables = await scheduleService.GetTimetablesLookupAsync();

        ViewData["NewRoomId"] = new SelectList(allNewRooms, "Id", "RoomNumber", scheduleChange.NewRoomId);
        ViewData["NewTeacherId"] = new SelectList(allNewTeachers, "Id", "FullName", scheduleChange.NewTeacherId);
        ViewData["TimetableId"] = new SelectList(allTimetables, "Id", "Text", scheduleChange.TimetableId);
        return View(scheduleChange);
    }

    // POST: ScheduleChanges/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("Id,TimetableId,ChangeDate,NewRoomId,NewTeacherId,NewStartTime,NewEndTime")]
        ScheduleChange scheduleChange)
    {
        if (id != scheduleChange.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await scheduleService.UpdateAsync(scheduleChange);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await scheduleService.ExistsAsync(scheduleChange.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        var allNewRooms = await scheduleService.GetAllRoomsAsync();
        var allNewTeachers = await scheduleService.GetTeachersLookupAsync();
        var allTimetables = await scheduleService.GetTimetablesLookupAsync();

        ViewData["NewRoomId"] = new SelectList(allNewRooms, "Id", "RoomNumber", scheduleChange.NewRoomId);
        ViewData["NewTeacherId"] = new SelectList(allNewTeachers, "Id", "FullName", scheduleChange.NewTeacherId);
        ViewData["TimetableId"] = new SelectList(allTimetables, "Id", "Text", scheduleChange.TimetableId);
        return View(scheduleChange);
    }

    // GET: ScheduleChanges/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var scheduleChange = await scheduleService.GetByIdWithRelationsAsync(id.Value);
        if (scheduleChange == null)
        {
            return NotFound();
        }

        return View(scheduleChange);
    }

    // POST: ScheduleChanges/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await scheduleService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}