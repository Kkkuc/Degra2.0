using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication.DTOs.Building;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers
{
    public class BuildingsController(IBuildingsService buildingsService) : Controller
    {
        // GET: Buildings
        public async Task<IActionResult> Index()
        {
            var data = await buildingsService.GetAllForIndexAsync();
            return View(data); 
        }

        // GET: Buildings/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var building = await buildingsService.GetDetailsByIdAsync(id);
            if (building == null)
            {
                return NotFound();
            }

            return View(building); 
        }

        // GET: Buildings/Create
        public async Task<IActionResult> Create()
        {
            var faculties = await buildingsService.GetFacultyDropdownListAsync();
            ViewData["FacultyId"] = new SelectList(faculties, "Key", "Value");
            return View();
        }

        // POST: Buildings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BuildingFormDto dto)
        {
            if (ModelState.IsValid)
            {
                await buildingsService.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            
            var faculties = await buildingsService.GetFacultyDropdownListAsync();
            ViewData["FacultyId"] = new SelectList(faculties, "Key", "Value", dto.FacultyId);
            return View(dto);
        }

        // GET: Buildings/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var buildingForm = await buildingsService.GetFormByIdAsync(id);
            if (buildingForm == null) return NotFound();

            var faculties = await buildingsService.GetFacultyDropdownListAsync();
            ViewData["FacultyId"] = new SelectList(faculties, "Key", "Value", buildingForm.FacultyId);
            return View(buildingForm);
        }

        // POST: Buildings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BuildingFormDto dto)
        {
            if (id != dto.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var success = await buildingsService.UpdateAsync(dto);
                if (!success)
                {
                    if (!await buildingsService.ExistsAsync(dto.Id)) return NotFound();
                    ModelState.AddModelError(string.Empty, "Wystąpił błąd aktualizacji.");
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            
            var faculties = await buildingsService.GetFacultyDropdownListAsync();
            ViewData["FacultyId"] = new SelectList(faculties, "Key", "Value", dto.FacultyId);
            return View(dto);
        }

        // GET: Buildings/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var building = await buildingsService.GetDetailsByIdAsync(id);
            if (building == null) return NotFound();

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
