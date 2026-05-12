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
    public class FieldsOfStudyController : Controller
    {
        private readonly AppDbContext _context;

        public FieldsOfStudyController(AppDbContext context)
        {
            _context = context;
        }

        // GET: FieldsOfStudy
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.FieldsOfStudy.Include(f => f.Faculty);
            return View(await appDbContext.ToListAsync());
        }

        // GET: FieldsOfStudy/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fieldOfStudy = await _context.FieldsOfStudy
                .Include(f => f.Faculty)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fieldOfStudy == null)
            {
                return NotFound();
            }

            return View(fieldOfStudy);
        }

        // GET: FieldsOfStudy/Create
        public IActionResult Create()
        {
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "Name");
            return View();
        }

        // POST: FieldsOfStudy/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FacultyId,Name,Degree,Mode")] FieldOfStudy fieldOfStudy)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fieldOfStudy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "Name", fieldOfStudy.FacultyId);
            return View(fieldOfStudy);
        }

        // GET: FieldsOfStudy/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fieldOfStudy = await _context.FieldsOfStudy.FindAsync(id);
            if (fieldOfStudy == null)
            {
                return NotFound();
            }
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "Name", fieldOfStudy.FacultyId);
            return View(fieldOfStudy);
        }

        // POST: FieldsOfStudy/Edit/5
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
                    _context.Update(fieldOfStudy);
                    await _context.SaveChangesAsync();
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
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "Name", fieldOfStudy.FacultyId);
            return View(fieldOfStudy);
        }

        // GET: FieldsOfStudy/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fieldOfStudy = await _context.FieldsOfStudy
                .Include(f => f.Faculty)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fieldOfStudy == null)
            {
                return NotFound();
            }

            return View(fieldOfStudy);
        }

        // POST: FieldsOfStudy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fieldOfStudy = await _context.FieldsOfStudy.FindAsync(id);
            if (fieldOfStudy != null)
            {
                _context.FieldsOfStudy.Remove(fieldOfStudy);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FieldOfStudyExists(int id)
        {
            return _context.FieldsOfStudy.Any(e => e.Id == id);
        }
    }
}
