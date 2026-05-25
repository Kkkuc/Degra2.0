using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Services;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers
{
    public class FieldsOfStudiesController(IFieldsOfStudiesService fieldsService) : Controller
    {
        // GET: FieldsOfStudies
        public async Task<IActionResult> Index()
        {
            var data = await fieldsService.GetAllWithFacultyAsync();
            return View(data);
        }

        // GET: FieldsOfStudies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fieldOfStudy = await fieldsService.GetByIdWithFacultyAsync(id.Value);
            if (fieldOfStudy == null)
            {
                return NotFound();
            }

            return View(fieldOfStudy);
        }

        // GET: FieldsOfStudies/Create
        public async Task<IActionResult> Create()
        {
            var faculties = await fieldsService.GetAllFacultiesAsync();
            ViewData["FacultyId"] = new SelectList(faculties, "Id", "Name");
            return View();
        }

        // POST: FieldsOfStudies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FacultyId,Name,Degree,Mode")] FieldOfStudy fieldOfStudy)
        {
            if (ModelState.IsValid)
            {
                await fieldsService.CreateAsync(fieldOfStudy);
                return RedirectToAction(nameof(Index));
            }

            var faculties = await fieldsService.GetAllFacultiesAsync();
            ViewData["FacultyId"] = new SelectList(faculties, "Id", "Name", fieldOfStudy.FacultyId);
            return View(fieldOfStudy);
        }

        // GET: FieldsOfStudies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fieldOfStudy = await fieldsService.GetByIdAsync(id.Value);
            if (fieldOfStudy == null)
            {
                return NotFound();
            }

            var faculties = await fieldsService.GetAllFacultiesAsync();
            ViewData["FacultyId"] = new SelectList(faculties, "Id", "Name", fieldOfStudy.FacultyId);
            return View(fieldOfStudy);
        }

        // POST: FieldsOfStudies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FacultyId,Name,Degree,Mode")] FieldOfStudy fieldOfStudy)
        {
            if (id != fieldOfStudy.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await fieldsService.UpdateAsync(fieldOfStudy);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await fieldsService.ExistsAsync(fieldOfStudy.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            var faculties = await fieldsService.GetAllFacultiesAsync();
            ViewData["FacultyId"] = new SelectList(faculties, "Id", "Name", fieldOfStudy.FacultyId);
            return View(fieldOfStudy);
        }

        // GET: FieldsOfStudies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fieldOfStudy = await fieldsService.GetByIdWithFacultyAsync(id.Value);
            if (fieldOfStudy == null)
            {
                return NotFound();
            }

            return View(fieldOfStudy);
        }

        // POST: FieldsOfStudies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await fieldsService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}