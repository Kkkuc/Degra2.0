using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers;

public class RoomsController(IRoomsService roomsService) : Controller
{
    // GET: Rooms
    public async Task<IActionResult> Index()
    {
        var data = await roomsService.GetAllWithBuildingAsync();
        return View(data);
    }

    // GET: Rooms/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var room = await roomsService.GetByIdWithBuildingAsync(id.Value);
        if (room == null)
        {
            return NotFound();
        }

        return View(room);
    }

    // GET: Rooms/Create
    public async Task<IActionResult> Create()
    {
        var buildings = await roomsService.GetAllBuildingsAsync();
        ViewData["BuildingId"] = new SelectList(buildings, "Id", "Name");
        return View();
    }

    // POST: Rooms/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,BuildingId,RoomNumber,Capacity,RoomType")] Room room)
    {
        if (ModelState.IsValid)
        {
            await roomsService.CreateAsync(room);
            return RedirectToAction(nameof(Index));
        }

        var buildings = await roomsService.GetAllBuildingsAsync();
        ViewData["BuildingId"] = new SelectList(buildings, "Id", "Name", room.BuildingId);
        return View(room);
    }

    // GET: Rooms/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var room = await roomsService.GetByIdAsync(id.Value);
        if (room == null)
        {
            return NotFound();
        }

        var buildings = await roomsService.GetAllBuildingsAsync();
        ViewData["BuildingId"] = new SelectList(buildings, "Id", "Name", room.BuildingId);
        return View(room);
    }

    // POST: Rooms/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,BuildingId,RoomNumber,Capacity,RoomType")] Room room)
    {
        if (id != room.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await roomsService.UpdateAsync(room);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await roomsService.ExistsAsync(room.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        var buildings = await roomsService.GetAllBuildingsAsync();
        ViewData["BuildingId"] = new SelectList(buildings, "Id", "Name", room.BuildingId);
        return View(room);
    }

    // GET: Rooms/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var room = await roomsService.GetByIdWithBuildingAsync(id.Value);
        if (room == null)
        {
            return NotFound();
        }

        return View(room);
    }

    // POST: Rooms/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await roomsService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}