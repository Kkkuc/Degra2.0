using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    public class SpecializationsController(ISpecializationsService specializationsService) : Controller
    {
        // GET: Specializations
        public async Task<IActionResult> Index()
        {
            var data = await specializationsService.GetAllAsync();
            return View(data);
        }

        // GET: Specializations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialization = await specializationsService.GetByIdAsync(id.Value);
            if (specialization == null)
            {
                return NotFound();
            }

            return View(specialization);
        }

        // GET: Specializations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Specializations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Specialization specialization)
        {
            if (!ModelState.IsValid)
            {
                return View(specialization);
            }

            await specializationsService.CreateAsync(specialization);
            return RedirectToAction(nameof(Index));
        }

        // GET: Specializations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialization = await specializationsService.GetByIdAsync(id.Value);
            if (specialization == null)
            {
                return NotFound();
            }

            return View(specialization);
        }

        // POST: Specializations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Specialization specialization)
        {
            if (id != specialization.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(specialization);
            }

            try
            {
                await specializationsService.UpdateAsync(specialization);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await specializationsService.ExistsAsync(specialization.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Specializations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialization = await specializationsService.GetByIdAsync(id.Value);
            if (specialization == null)
            {
                return NotFound();
            }

            return View(specialization);
        }

        // POST: Specializations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await specializationsService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}