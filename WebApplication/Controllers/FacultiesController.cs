using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.DTOs.Faculty;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers
{
    public class FacultiesController(IFacultiesService facultiesService) : Controller
    {
        // GET: Faculties
        public async Task<IActionResult> Index()
        {
            var data = await facultiesService.GetAllAsync();
            return View(data);
        }

        // GET: Faculties/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var dto = await facultiesService.GetByIdAsync(id);
            if (dto == null)
            {
                return NotFound();
            }

            return View(dto);
        }

        // GET: Faculties/Create
        public IActionResult Create() => View();

        // POST: Faculties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FacultyDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await facultiesService.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        // GET: Faculties/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await facultiesService.GetByIdAsync(id);
            if (dto == null)
            {
                return NotFound();
            }

            return View(dto);
        }

        // POST: Faculties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FacultyDto dto)
        {
            if (id != dto.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var updated = await facultiesService.UpdateAsync(dto);
            if (updated)
            {
                return RedirectToAction(nameof(Index));
            }

            if (!await facultiesService.ExistsAsync(dto.Id))
            {
                return NotFound();
            }

            ModelState.AddModelError(string.Empty, "Error during updating");
            return View(dto);
        }

        // GET: Faculties/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var dto = await facultiesService.GetByIdAsync(id);
            if (dto == null)
            {
                return NotFound();
            }

            return View(dto);
        }

        // POST: Faculties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await facultiesService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}