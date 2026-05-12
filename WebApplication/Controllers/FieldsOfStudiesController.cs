using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class FieldsOfStudiesController(AppDbContext context) : Controller
    {
        // GET: FieldsOfStudies
        public async Task<IActionResult> Index()
        {
            var appDbContext = context.FieldsOfStudy.Include(f => f.Faculty);
            return View(await appDbContext.ToListAsync());
        }

        // GET: FieldsOfStudies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fieldOfStudy = await context.FieldsOfStudy
                .Include(f => f.Faculty)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fieldOfStudy == null)
            {
                return NotFound();
            }

            return View(fieldOfStudy);
        }

        // GET: FieldsOfStudies/Create
        public IActionResult Create()
        {
            ViewData["FacultyId"] = new SelectList(context.Faculties, "Id", "Name");
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
                context.Add(fieldOfStudy);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FacultyId"] = new SelectList(context.Faculties, "Id", "Name", fieldOfStudy.FacultyId);
            return View(fieldOfStudy);
        }

        // GET: FieldsOfStudies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fieldOfStudy = await context.FieldsOfStudy.FindAsync(id);
            if (fieldOfStudy == null)
            {
                return NotFound();
            }
            ViewData["FacultyId"] = new SelectList(context.Faculties, "Id", "Name", fieldOfStudy.FacultyId);
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
                    context.Update(fieldOfStudy);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FieldOfStudyExists(fieldOfStudy.Id))
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
            ViewData["FacultyId"] = new SelectList(context.Faculties, "Id", "Name", fieldOfStudy.FacultyId);
            return View(fieldOfStudy);
        }

        // GET: FieldsOfStudies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fieldOfStudy = await context.FieldsOfStudy
                .Include(f => f.Faculty)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var fieldOfStudy = await context.FieldsOfStudy.FindAsync(id);
            if (fieldOfStudy != null)
            {
                context.FieldsOfStudy.Remove(fieldOfStudy);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FieldOfStudyExists(int id)
        {
            return context.FieldsOfStudy.Any(e => e.Id == id);
        }
    }
}
