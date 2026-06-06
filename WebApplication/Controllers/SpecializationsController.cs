using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.DTOs.Specialization;
using WebApplication.Models;
using WebApplication.Services.Interfaces;

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
        public async Task<IActionResult> Create(SpecializationDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await specializationsService.CreateAsync(dto);
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
        public async Task<IActionResult> Edit(int id, SpecializationDto dto)
        {
            if (id != dto.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var success = await specializationsService.UpdateAsync(dto);
            if (!success)
            {
                if (!await specializationsService.ExistsAsync(dto.Id))
                {
                    return NotFound();
                }
            
                ModelState.AddModelError(string.Empty, "Wystąpił nieoczekiwany błąd podczas aktualizacji.");
                return View(dto);
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
            var success = await specializationsService.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}