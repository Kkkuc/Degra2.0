using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    public class BuildingsController(IBuildingsService buildingsService) : Controller
    {
        // GET: Buildings
        public async Task<IActionResult> Index()
        {
            var data = await buildingsService.GetAllWithFacultyAsync();
            return View(data);
        }

        // GET: Buildings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var building = await buildingsService.GetByIdWithFacultyAsync(id.Value);
            if (building == null)
            {
                return NotFound();
            }

            return View(building);
        }

        // GET: Buildings/Create
        public async Task<IActionResult> Create()
        {
            var faculties = await buildingsService.GetAllFacultiesAsync();
            ViewData["FacultyId"] = new SelectList(faculties, "Id", "Name");
            return View();
        }

        // POST: Buildings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Street,HouseNumber,City,PostalCode,FacultyId")] Building building)
        {
            if (ModelState.IsValid)
            {
                await buildingsService.CreateAsync(building);
                return RedirectToAction(nameof(Index));
            }
            var faculties = await buildingsService.GetAllFacultiesAsync();
            ViewData["FacultyId"] = new SelectList(faculties, "Id", "Name", building.FacultyId);
            return View(building);
        }

        // GET: Buildings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var building = await buildingsService.GetByIdAsync(id.Value);
            if (building == null)
            {
                return NotFound();
            }
            var faculties = await buildingsService.GetAllFacultiesAsync();
            ViewData["FacultyId"] = new SelectList(faculties, "Id", "Name", building.FacultyId);
            return View(building);
        }

        // POST: Buildings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Street,HouseNumber,City,PostalCode,FacultyId")] Building building)
        {
            if (id != building.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await buildingsService.UpdateAsync(building);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await buildingsService.ExistsAsync(building.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            var faculties = await buildingsService.GetAllFacultiesAsync();
            ViewData["FacultyId"] = new SelectList(faculties, "Id", "Name", building.FacultyId);
            return View(building);
        }

        // GET: Buildings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var building = await buildingsService.GetByIdWithFacultyAsync(id.Value);
            if (building == null)
            {
                return NotFound();
            }

            return View(building);
        }

        // POST: Buildings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await buildingsService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        
    }
}
