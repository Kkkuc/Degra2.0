using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class StudentGroupController(AppDbContext context) : Controller
    {
        // GET: StudentGroup
        public async Task<IActionResult> Index()
        {
            var appDbContext = context.StudentGroups.Include(s => s.Group).Include(s => s.Student);
            return View(await appDbContext.ToListAsync());
        }

        // GET: StudentGroup/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentGroup = await context.StudentGroups
                .Include(s => s.Group)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (studentGroup == null)
            {
                return NotFound();
            }

            return View(studentGroup);
        }

        // GET: StudentGroup/Create
        public IActionResult Create()
        {
            ViewData["GroupId"] = new SelectList(context.Groups, "Id", "Name");
            ViewData["StudentId"] = new SelectList(context.Students, "Id", "FirstName");
            return View();
        }

        // POST: StudentGroup/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentId,GroupId")] StudentGroup studentGroup)
        {
            if (ModelState.IsValid)
            {
                context.Add(studentGroup);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GroupId"] = new SelectList(context.Groups, "Id", "Name", studentGroup.GroupId);
            ViewData["StudentId"] = new SelectList(context.Students, "Id", "FirstName", studentGroup.StudentId);
            return View(studentGroup);
        }

        // GET: StudentGroup/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentGroup = await context.StudentGroups.FindAsync(id);
            if (studentGroup == null)
            {
                return NotFound();
            }
            ViewData["GroupId"] = new SelectList(context.Groups, "Id", "Name", studentGroup.GroupId);
            ViewData["StudentId"] = new SelectList(context.Students, "Id", "FirstName", studentGroup.StudentId);
            return View(studentGroup);
        }

        // POST: StudentGroup/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StudentId,GroupId")] StudentGroup studentGroup)
        {
            if (id != studentGroup.StudentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(studentGroup);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentGroupExists(studentGroup.StudentId))
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
            ViewData["GroupId"] = new SelectList(context.Groups, "Id", "Name", studentGroup.GroupId);
            ViewData["StudentId"] = new SelectList(context.Students, "Id", "FirstName", studentGroup.StudentId);
            return View(studentGroup);
        }

        // GET: StudentGroup/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentGroup = await context.StudentGroups
                .Include(s => s.Group)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (studentGroup == null)
            {
                return NotFound();
            }

            return View(studentGroup);
        }

        // POST: StudentGroup/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentGroup = await context.StudentGroups.FindAsync(id);
            if (studentGroup != null)
            {
                context.StudentGroups.Remove(studentGroup);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentGroupExists(int id)
        {
            return context.StudentGroups.Any(e => e.StudentId == id);
        }
    }
}
