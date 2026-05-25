using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Models;
using WebApplication.Services;

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
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faculty = await facultiesService.GetByIdAsync(id.Value);
            if (faculty == null)
            {
                return NotFound();
            }

            return View(faculty);
        }

        // GET: Faculties/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Faculties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Abbreviation")] Faculty faculty)
        {
            if (!ModelState.IsValid)
            {
                return View(faculty);
            }
            await facultiesService.CreateAsync(faculty);
            return RedirectToAction(nameof(Index));
        }

        // GET: Faculties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faculty = await facultiesService.GetByIdAsync(id.Value);
            if (faculty == null)
            {
                return NotFound();
            }
            return View(faculty);
        }

        // POST: Faculties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Abbreviation")] Faculty faculty)
        {
            if (id != faculty.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(faculty);
            }
            try
            {
                await facultiesService.UpdateAsync(faculty);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await facultiesService.ExistsAsync(faculty.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Faculties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faculty = await facultiesService.GetByIdAsync(id.Value);
            if (faculty == null)
            {
                return NotFound();
            }

            return View(faculty);
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
