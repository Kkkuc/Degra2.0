using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class GroupsController(AppDbContext context) : Controller
    {
        // GET: Groups
        public async Task<IActionResult> Index()
        {
            var appDbContext = context.Groups
                .Include(g => g.FieldOfStudy)
                .Include(g => g.Semester)
                .Include(g => g.Specialization);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Groups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await context.Groups
                .Include(g => g.FieldOfStudy)
                .Include(g => g.Semester)
                .Include(g => g.Specialization)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@group == null)
            {
                return NotFound();
            }

            return View(@group);
        }

        // GET: Groups/Create
        public IActionResult Create()
        {
            ViewData["FieldOfStudyId"] = new SelectList(context.FieldsOfStudy, "Id", "Name");
            ViewData["SemesterId"] = new SelectList(context.Semesters, "Id", "Name");
            ViewData["SpecializationId"] = new SelectList(context.Specializations, "Id", "Name");
            return View();
        }

        // POST: Groups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SemesterId,FieldOfStudyId,SpecializationId,ClassType,Name")] Group @group)
        {
            if (ModelState.IsValid)
            {
                context.Add(@group);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FieldOfStudyId"] = new SelectList(context.FieldsOfStudy, "Id", "Name", @group.FieldOfStudyId);
            ViewData["SemesterId"] = new SelectList(context.Semesters, "Id", "Name", @group.SemesterId);
            ViewData["SpecializationId"] = new SelectList(context.Specializations, "Id", "Name", @group.SpecializationId);
            return View(@group);
        }

        // GET: Groups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await context.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }
            ViewData["FieldOfStudyId"] = new SelectList(context.FieldsOfStudy, "Id", "Name", group.FieldOfStudyId);
            ViewData["SemesterId"] = new SelectList(context.Semesters, "Id", "Name", group.SemesterId);
            ViewData["SpecializationId"] = new SelectList(context.Specializations, "Id", "Name", group.SpecializationId);
            return View(group);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SemesterId,FieldOfStudyId,SpecializationId,ClassType,Name")] Group @group)
        {
            if (id != @group.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(@group);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(@group.Id))
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
            ViewData["FieldOfStudyId"] = new SelectList(context.FieldsOfStudy, "Id", "Name", @group.FieldOfStudyId);
            ViewData["SemesterId"] = new SelectList(context.Semesters, "Id", "Name", @group.SemesterId);
            ViewData["SpecializationId"] = new SelectList(context.Specializations, "Id", "Name", @group.SpecializationId);
            return View(@group);
        }

        // GET: Groups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await context.Groups
                .Include(g => g.FieldOfStudy)
                .Include(g => g.Semester)
                .Include(g => g.Specialization)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (group == null)
            {
                return NotFound();
            }

            return View(group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @group = await context.Groups.FindAsync(id);
            if (@group != null)
            {
                context.Groups.Remove(@group);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupExists(int id)
        {
            return context.Groups.Any(e => e.Id == id);
        }
    }
}
