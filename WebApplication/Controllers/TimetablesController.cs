using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.DTOs.Timetable;
using WebApplication.Models.enums;
using WebApplication.Services.Interfaces;

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
    public async Task<IActionResult> Create(TimetableCreateDto timetableDto)
    {
        if (ModelState.IsValid)
        {
            await timetablesService.CreateAsync(timetableDto);
            return RedirectToAction(nameof(Index));
        }
        var groups = await timetablesService.GetAllGroupsAsync();
        var rooms = await timetablesService.GetAllRoomsAsync();
        var subjects = await timetablesService.GetAllSubjectsAsync();
        var teachers = await timetablesService.GetAllTeachersAsync();

        ViewData["GroupId"] = new SelectList(groups, "Id", "Name", timetableDto.GroupId);
        ViewData["RoomId"] = new SelectList(rooms, "Id", "RoomNumber", timetableDto.RoomId);
        ViewData["SubjectId"] = new SelectList(subjects, "Id", "Name", timetableDto.SubjectId);
        ViewData["TeacherId"] = new SelectList(teachers, "Id", "FirstName", timetableDto.TeacherId);
        return View(timetableDto);
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
        ViewData["ClassType"] = new SelectList(Enum.GetValues<ClassType>());
        ViewData["DayOfWeek"] = new SelectList(Enum.GetValues<DayOfWeek>());
        ViewData["WeekCycle"] = new SelectList(Enum.GetValues<WeekCycle>());
        return View(timetable);
    }

    // POST: Timetables/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TimetableEditDto timetableDto)
    {
        if (id != timetableDto.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
               
               await timetablesService.UpdateAsync(timetableDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await timetablesService.ExistsAsync(timetableDto.Id))
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

        // ZMIANA: Przypisanie wybranych wartości z obiektu DTO
        ViewData["GroupId"] = new SelectList(groups, "Id", "Name", timetableDto.GroupId);
        ViewData["RoomId"] = new SelectList(rooms, "Id", "RoomNumber", timetableDto.RoomId);
        ViewData["SubjectId"] = new SelectList(subjects, "Id", "Name", timetableDto.SubjectId);
        ViewData["TeacherId"] = new SelectList(teachers, "Id", "FirstName", timetableDto.TeacherId);
        ViewData["ClassType"] = new SelectList(Enum.GetValues<ClassType>());
        ViewData["DayOfWeek"] = new SelectList(Enum.GetValues<DayOfWeek>());
        ViewData["WeekCycle"] = new SelectList(Enum.GetValues<WeekCycle>());
        return View(timetableDto);
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