using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class BuildingsController(AppDbContext context) : Controller
    {
        // GET: Buildings
        public async Task<IActionResult> Index()
        {
            var appDbContext = context.Buildings.Include(b => b.Faculty);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Buildings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var building = await context.Buildings
                .Include(b => b.Faculty)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (building == null)
            {
                return NotFound();
            }

            return View(building);
        }

        // GET: Buildings/Create
        public IActionResult Create()
        {
            ViewData["FacultyId"] = new SelectList(context.Faculties, "Id", "Name");
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
                context.Add(building);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FacultyId"] = new SelectList(context.Faculties, "Id", "Name", building.FacultyId);
            return View(building);
        }

        // GET: Buildings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var building = await context.Buildings.FindAsync(id);
            if (building == null)
            {
                return NotFound();
            }
            ViewData["FacultyId"] = new SelectList(context.Faculties, "Id", "Name", building.FacultyId);
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
                    context.Update(building);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BuildingExists(building.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FacultyId"] = new SelectList(context.Faculties, "Id", "Name", building.FacultyId);
            return View(building);
        }

        // GET: Buildings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var building = await context.Buildings
                .Include(b => b.Faculty)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var building = await context.Buildings.FindAsync(id);
            if (building != null)
            {
                context.Buildings.Remove(building);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BuildingExists(int id)
        {
            return context.Buildings.Any(e => e.Id == id);
        }
    }
}
