using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class ScheduleChangesController(AppDbContext context) : Controller
    {
        // GET: ScheduleChanges
        public async Task<IActionResult> Index()
        {
            var appDbContext = context.ScheduleChanges.Include(s => s.NewRoom).Include(s => s.NewTeacher).Include(s => s.OriginalEntry);
            return View(await appDbContext.ToListAsync());
        }

        // GET: ScheduleChanges/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleChange = await context.ScheduleChanges
                .Include(s => s.NewRoom)
                .Include(s => s.NewTeacher)
                .Include(s => s.OriginalEntry)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (scheduleChange == null)
            {
                return NotFound();
            }

            return View(scheduleChange);
        }

        // GET: ScheduleChanges/Create
        public IActionResult Create()
        {
            ViewData["NewRoomId"] = new SelectList(context.Rooms, "Id", "RoomNumber");
            ViewData["NewTeacherId"] = new SelectList(context.Teachers, "Id", "FirstName");
            ViewData["TimetableId"] = new SelectList(context.Timetables, "Id", "Id");
            return View();
        }

        // POST: ScheduleChanges/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TimetableId,ChangeDate,ChangeType,NewRoomId,NewTeacherId,NewStartTime,NewEndTime")] ScheduleChange scheduleChange)
        {
            if (ModelState.IsValid)
            {
                context.Add(scheduleChange);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NewRoomId"] = new SelectList(context.Rooms, "Id", "RoomNumber", scheduleChange.NewRoomId);
            ViewData["NewTeacherId"] = new SelectList(context.Teachers, "Id", "FirstName", scheduleChange.NewTeacherId);
            ViewData["TimetableId"] = new SelectList(context.Timetables, "Id", "Id", scheduleChange.TimetableId);
            return View(scheduleChange);
        }

        // GET: ScheduleChanges/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleChange = await context.ScheduleChanges.FindAsync(id);
            if (scheduleChange == null)
            {
                return NotFound();
            }
            ViewData["NewRoomId"] = new SelectList(context.Rooms, "Id", "RoomNumber", scheduleChange.NewRoomId);
            ViewData["NewTeacherId"] = new SelectList(context.Teachers, "Id", "FirstName", scheduleChange.NewTeacherId);
            ViewData["TimetableId"] = new SelectList(context.Timetables, "Id", "Id", scheduleChange.TimetableId);
            return View(scheduleChange);
        }

        // POST: ScheduleChanges/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TimetableId,ChangeDate,ChangeType,NewRoomId,NewTeacherId,NewStartTime,NewEndTime")] ScheduleChange scheduleChange)
        {
            if (id != scheduleChange.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(scheduleChange);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleChangeExists(scheduleChange.Id))
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
            ViewData["NewRoomId"] = new SelectList(context.Rooms, "Id", "RoomNumber", scheduleChange.NewRoomId);
            ViewData["NewTeacherId"] = new SelectList(context.Teachers, "Id", "FirstName", scheduleChange.NewTeacherId);
            ViewData["TimetableId"] = new SelectList(context.Timetables, "Id", "Id", scheduleChange.TimetableId);
            return View(scheduleChange);
        }

        // GET: ScheduleChanges/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleChange = await context.ScheduleChanges
                .Include(s => s.NewRoom)
                .Include(s => s.NewTeacher)
                .Include(s => s.OriginalEntry)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (scheduleChange == null)
            {
                return NotFound();
            }

            return View(scheduleChange);
        }

        // POST: ScheduleChanges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var scheduleChange = await context.ScheduleChanges.FindAsync(id);
            if (scheduleChange != null)
            {
                context.ScheduleChanges.Remove(scheduleChange);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScheduleChangeExists(int id)
        {
            return context.ScheduleChanges.Any(e => e.Id == id);
        }
    }
}
