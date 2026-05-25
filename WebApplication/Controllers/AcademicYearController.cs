using Microsoft.AspNetCore.Mvc;
using WebApplication.DTOs;
using WebApplication.DTOs.AcademicYear;
using WebApplication.Services;
using WebApplication.Services.Interfaces;

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
        public async Task<IActionResult> Details(int id)
        {
            var dto = await academicYearService.GetByIdAsync(id);
            if (dto == null)
            {
                return NotFound();
            }

            return View(dto);
        }

        // GET: AcademicYear/Create
        public IActionResult Create() => View();


        // POST: AcademicYear/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AcademicYearFormDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await academicYearService.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        // GET: AcademicYear/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var dto = await academicYearService.GetByIdAsync(id);
            if (dto == null)
            {
                return NotFound();
            }

            return View(dto);
        }

        // POST: AcademicYear/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AcademicYearFormDto dto)
        {
            if (id != dto.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var updated = await academicYearService.UpdateAsync(dto);
            if (!updated)
            {
                return NotFound();
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