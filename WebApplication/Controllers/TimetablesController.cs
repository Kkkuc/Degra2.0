using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers;

public class TimetablesController(ITimetablesService timetablesService) : Controller
{
    // GET: Timetables
    public async Task<IActionResult> Index()
    {
        var data = await timetablesService.GetAllWithRelationsAsync();
        return View(data);
    }

    // GET: Timetables/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var timetable = await timetablesService.GetByIdWithRelationsAsync(id.Value);
        if (timetable == null)
        {
            return NotFound();
        }

        return View(timetable);
    }

    // GET: Timetables/Create
    public async Task<IActionResult> Create()
    {
        var groups = await timetablesService.GetAllGroupsAsync();
        var rooms = await timetablesService.GetAllRoomsAsync();
        var subjects = await timetablesService.GetAllSubjectsAsync();
        var teachers = await timetablesService.GetAllTeachersAsync();

        ViewData["GroupId"] = new SelectList(groups, "Id", "Name");
        ViewData["RoomId"] = new SelectList(rooms, "Id", "RoomNumber");
        ViewData["SubjectId"] = new SelectList(subjects, "Id", "Name");
        ViewData["TeacherId"] = new SelectList(teachers, "Id", "FirstName");
        return View();
    }

    // POST: Timetables/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,SubjectId,TeacherId,RoomId,GroupId,ClassType,DayOfWeek,StartTime,EndTime,WeekCycle")] Timetable timetable)
    {
        if (ModelState.IsValid)
        {
            await timetablesService.CreateAsync(timetable);
            return RedirectToAction(nameof(Index));
        }
        var groups = await timetablesService.GetAllGroupsAsync();
        var rooms = await timetablesService.GetAllRoomsAsync();
        var subjects = await timetablesService.GetAllSubjectsAsync();
        var teachers = await timetablesService.GetAllTeachersAsync();

        ViewData["GroupId"] = new SelectList(groups, "Id", "Name", timetable.GroupId);
        ViewData["RoomId"] = new SelectList(rooms, "Id", "RoomNumber", timetable.RoomId);
        ViewData["SubjectId"] = new SelectList(subjects, "Id", "Name", timetable.SubjectId);
        ViewData["TeacherId"] = new SelectList(teachers, "Id", "FirstName", timetable.TeacherId);
        return View(timetable);
    }

    // GET: Timetables/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var timetable = await timetablesService.GetByIdAsync(id.Value);
        if (timetable == null)
        {
            return NotFound();
        }
        var groups = await timetablesService.GetAllGroupsAsync();
        var rooms = await timetablesService.GetAllRoomsAsync();
        var subjects = await timetablesService.GetAllSubjectsAsync();
        var teachers = await timetablesService.GetAllTeachersAsync();

        ViewData["GroupId"] = new SelectList(groups, "Id", "Name", timetable.GroupId);
        ViewData["RoomId"] = new SelectList(rooms, "Id", "RoomNumber", timetable.RoomId);
        ViewData["SubjectId"] = new SelectList(subjects, "Id", "Name", timetable.SubjectId);
        ViewData["TeacherId"] = new SelectList(teachers, "Id", "FirstName", timetable.TeacherId);
        return View(timetable);
    }

    // POST: Timetables/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,SubjectId,TeacherId,RoomId,GroupId,ClassType,DayOfWeek,StartTime,EndTime,WeekCycle")] Timetable timetable)
    {
        if (id != timetable.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await timetablesService.UpdateAsync(timetable);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await timetablesService.ExistsAsync(timetable.Id))
                {
                    return NotFound();
                }

                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        var groups = await timetablesService.GetAllGroupsAsync();
        var rooms = await timetablesService.GetAllRoomsAsync();
        var subjects = await timetablesService.GetAllSubjectsAsync();
        var teachers = await timetablesService.GetAllTeachersAsync();

        ViewData["GroupId"] = new SelectList(groups, "Id", "Name", timetable.GroupId);
        ViewData["RoomId"] = new SelectList(rooms, "Id", "RoomNumber", timetable.RoomId);
        ViewData["SubjectId"] = new SelectList(subjects, "Id", "Name", timetable.SubjectId);
        ViewData["TeacherId"] = new SelectList(teachers, "Id", "FirstName", timetable.TeacherId);
        return View(timetable);
    }

    // GET: Timetables/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var timetable = await timetablesService.GetByIdWithRelationsAsync(id.Value);
        if (timetable == null)
        {
            return NotFound();
        }

        return View(timetable);
    }

    // POST: Timetables/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await timetablesService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}