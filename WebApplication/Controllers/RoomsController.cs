using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication.DTOs.Room;
using WebApplication.Models.enums;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

public class RoomsController(IRoomsService roomsService) : Controller
{
    // GET: Rooms
    public async Task<IActionResult> Index()
    {
        var data = await roomsService.GetAllForIndexAsync();
        return View(data);
    }

    // GET: Rooms/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var room = await roomsService.GetDetailsByIdAsync(id);
        if (room == null)
        {
            return NotFound();
        }

        return View(room);
    }

    // GET: Rooms/Create
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View();
    }

    // POST: Rooms/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RoomFormDto dto)
    {
        if (ModelState.IsValid)
        {
            await roomsService.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        await PopulateDropdownsAsync(dto);
        return View(dto);
    }

    // GET: Rooms/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var roomForm = await roomsService.GetFormByIdAsync(id);
        if (roomForm == null)
        {
            return NotFound();
        }

        await PopulateDropdownsAsync(roomForm);
        return View(roomForm);
    }

    // POST: Rooms/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RoomFormDto dto)
    {
        if (id != dto.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var success = await roomsService.UpdateAsync(dto);
            if (!success)
            {
                if (!await roomsService.ExistsAsync(dto.Id))
                {
                    return NotFound();
                }
                ModelState.AddModelError(string.Empty, "Wystąpił błąd aktualizacji sali.");
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        await PopulateDropdownsAsync(dto);
        return View(dto);
    }

    // GET: Rooms/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var room = await roomsService.GetDetailsByIdAsync(id);
        if (room == null) return NotFound();

        return View(room);
    }

    // POST: Rooms/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var success = await roomsService.DeleteAsync(id);
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
    
    private async Task PopulateDropdownsAsync(RoomFormDto? dto = null)
    {
        var buildings = await roomsService.GetBuildingsDropdownListAsync();
        ViewData["BuildingId"] = new SelectList(buildings, "Key", "Value", dto?.BuildingId);
        
        ViewData["RoomType"] = dto != null
            ? new SelectList(Enum.GetValues<RoomType>(), dto.RoomType)
            : new SelectList(Enum.GetValues<RoomType>());
    }
}