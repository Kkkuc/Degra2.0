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
    public class AcademicYearController(AppDbContext context) : Controller
    {
        // GET: AcademicYear
        public async Task<IActionResult> Index()
        {
            return View(await context.AcademicYears.ToListAsync());
        }

        // GET: AcademicYear/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicYear = await context.AcademicYears
                .FirstOrDefaultAsync(m => m.Id == id);
            if (academicYear == null)
            {
                return NotFound();
            }

            return View(academicYear);
        }

        // GET: AcademicYear/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AcademicYear/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,StartDate,EndDate")] AcademicYear academicYear)
        {
            if (ModelState.IsValid)
            {
                context.Add(academicYear);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(academicYear);
        }

        // GET: AcademicYear/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicYear = await context.AcademicYears.FindAsync(id);
            if (academicYear == null)
            {
                return NotFound();
            }
            return View(academicYear);
        }

        // POST: AcademicYear/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartDate,EndDate")] AcademicYear academicYear)
        {
            if (id != academicYear.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(academicYear);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AcademicYearExists(academicYear.Id))
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
            return View(academicYear);
        }

        // GET: AcademicYear/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicYear = await context.AcademicYears
                .FirstOrDefaultAsync(m => m.Id == id);
            if (academicYear == null)
            {
                return NotFound();
            }

            return View(academicYear);
        }

        // POST: AcademicYear/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var academicYear = await context.AcademicYears.FindAsync(id);
            if (academicYear != null)
            {
                context.AcademicYears.Remove(academicYear);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AcademicYearExists(int id)
        {
            return context.AcademicYears.Any(e => e.Id == id);
        }
    }
}
