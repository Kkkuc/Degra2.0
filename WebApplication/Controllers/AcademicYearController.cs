using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    public class AcademicYearController(IAcademicYearService academicYearService) : Controller
    {
        // GET: AcademicYear
        public async Task<IActionResult> Index()
        {
            var data = await academicYearService.GetAllAsync();
            return View(data);
        }

        // GET: AcademicYear/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var academicYear = await academicYearService.GetByIdAsync(id.Value);
            if (academicYear == null) return NotFound();

            return View(academicYear);
        }

        // GET: AcademicYear/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AcademicYear/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,StartDate,EndDate")] AcademicYear academicYear)
        {
            if (!ModelState.IsValid)
            {
                return View(academicYear);
            }

            await academicYearService.CreateAsync(academicYear);
            return RedirectToAction(nameof(Index));
        }

        // GET: AcademicYear/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicYear = await academicYearService.GetByIdAsync(id.Value);
            if (academicYear == null)
            {
                return NotFound();
            }

            return View(academicYear);
        }

        // POST: AcademicYear/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartDate,EndDate")] AcademicYear academicYear)
        {
            if (id != academicYear.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(academicYear);
            }

            try
            {
                await academicYearService.UpdateAsync(academicYear);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await academicYearService.ExistsAsync(academicYear.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: AcademicYear/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicYear = await academicYearService.GetByIdAsync(id.Value);
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
            await academicYearService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}