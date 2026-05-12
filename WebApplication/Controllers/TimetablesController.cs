using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class TimetablesController(AppDbContext context) : Controller
    {
        // GET: Timetables
        public async Task<IActionResult> Index()
        {
            var appDbContext = context.Timetables.Include(t => t.Group).Include(t => t.Room).Include(t => t.Subject).Include(t => t.Teacher);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Timetables/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timetable = await context.Timetables
                .Include(t => t.Group)
                .Include(t => t.Room)
                .Include(t => t.Subject)
                .Include(t => t.Teacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (timetable == null)
            {
                return NotFound();
            }

            return View(timetable);
        }

        // GET: Timetables/Create
        public IActionResult Create()
        {
            ViewData["GroupId"] = new SelectList(context.Groups, "Id", "Name");
            ViewData["RoomId"] = new SelectList(context.Rooms, "Id", "RoomNumber");
            ViewData["SubjectId"] = new SelectList(context.Subjects, "Id", "Name");
            ViewData["TeacherId"] = new SelectList(context.Teachers, "Id", "FirstName");
            return View();
        }

        // POST: Timetables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SubjectId,TeacherId,RoomId,GroupId,ClassType,DayOfWeek,StartTime,EndTime,WeekCycle")] Timetable timetable)
        {
            if (ModelState.IsValid)
            {
                context.Add(timetable);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GroupId"] = new SelectList(context.Groups, "Id", "Name", timetable.GroupId);
            ViewData["RoomId"] = new SelectList(context.Rooms, "Id", "RoomNumber", timetable.RoomId);
            ViewData["SubjectId"] = new SelectList(context.Subjects, "Id", "Name", timetable.SubjectId);
            ViewData["TeacherId"] = new SelectList(context.Teachers, "Id", "FirstName", timetable.TeacherId);
            return View(timetable);
        }

        // GET: Timetables/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timetable = await context.Timetables.FindAsync(id);
            if (timetable == null)
            {
                return NotFound();
            }
            ViewData["GroupId"] = new SelectList(context.Groups, "Id", "Name", timetable.GroupId);
            ViewData["RoomId"] = new SelectList(context.Rooms, "Id", "RoomNumber", timetable.RoomId);
            ViewData["SubjectId"] = new SelectList(context.Subjects, "Id", "Name", timetable.SubjectId);
            ViewData["TeacherId"] = new SelectList(context.Teachers, "Id", "FirstName", timetable.TeacherId);
            return View(timetable);
        }

        // POST: Timetables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SubjectId,TeacherId,RoomId,GroupId,ClassType,DayOfWeek,StartTime,EndTime,WeekCycle")] Timetable timetable)
        {
            if (id != timetable.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(timetable);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TimetableExists(timetable.Id))
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
            ViewData["GroupId"] = new SelectList(context.Groups, "Id", "Name", timetable.GroupId);
            ViewData["RoomId"] = new SelectList(context.Rooms, "Id", "RoomNumber", timetable.RoomId);
            ViewData["SubjectId"] = new SelectList(context.Subjects, "Id", "Name", timetable.SubjectId);
            ViewData["TeacherId"] = new SelectList(context.Teachers, "Id", "FirstName", timetable.TeacherId);
            return View(timetable);
        }

        // GET: Timetables/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timetable = await context.Timetables
                .Include(t => t.Group)
                .Include(t => t.Room)
                .Include(t => t.Subject)
                .Include(t => t.Teacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (timetable == null)
            {
                return NotFound();
            }

            return View(timetable);
        }

        // POST: Timetables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var timetable = await context.Timetables.FindAsync(id);
            if (timetable != null)
            {
                context.Timetables.Remove(timetable);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TimetableExists(int id)
        {
            return context.Timetables.Any(e => e.Id == id);
        }
    }
}
