using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers;

public class SchedulerController : Controller
{
    private readonly AppDbContext _context;

    public SchedulerController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var timetables = await _context.Timetables
            .Include(t => t.Subject)
            .Include(t => t.Teacher)
            .Include(t => t.Room)
            .ToListAsync();
        var subjects = await _context.Subjects.ToListAsync();

        var viewModel = new SchedulerViewModel
        {
            Lessons = timetables.Select(t => new TimetableEntryDto
            {
                Id = t.Id.ToString(),
                SubjectId = t.SubjectId, // Map the ID here
                Subject = t.Subject.Name,
                Color = GetColorForSubject(t.Subject.Id),
                Day = t.DayOfWeek - 1,
                StartSlot = GetSlotIndex(t.StartTime),
                Duration = 2,
                Room = t.Room.RoomNumber,
                Teacher = $"{t.Teacher.FirstName} {t.Teacher.LastName}",
                Time = $"{t.StartTime:hh\\:mm} – {t.EndTime:hh\\:mm}"
            }).ToList(),
            Subjects = subjects.Select(s => new SubjectDto
            {
                Id = s.Id.ToString(),
                Name = s.Name,
                Color = GetColorForSubject(s.Id)
            }).ToList(),
            TimeSlots = new List<string>
            {
                "08:30 – 09:15", "09:15 – 10:00", "10:15 – 11:00", "11:00 – 11:45",
                "12:00 – 12:45", "12:45 – 13:30", "14:00 – 14:45", "14:45 – 15:30",
                "16:00 – 16:45", "16:45 – 17:30", "17:40 – 18:25", "18:25 – 19:10"
            }
        };
        return View(viewModel);
    }

    private string GetColorForSubject(int id)
    {
        string[] colors = { "#EF4444", "#F97316", "#22C55E", "#3B82F6", "#8B5CF6", "#EC4899" };
        return colors[id % colors.Length];
    }

    private int GetSlotIndex(TimeSpan time)
    {
        if (time >= new TimeSpan(18, 25, 0)) return 11;
        if (time >= new TimeSpan(17, 40, 0)) return 10;
        if (time >= new TimeSpan(16, 45, 0)) return 9;
        if (time >= new TimeSpan(16, 0, 0)) return 8;
        if (time >= new TimeSpan(14, 45, 0)) return 7;
        if (time >= new TimeSpan(14, 0, 0)) return 6;
        if (time >= new TimeSpan(12, 45, 0)) return 5;
        if (time >= new TimeSpan(12, 0, 0)) return 4;
        if (time >= new TimeSpan(11, 0, 0)) return 3;
        if (time >= new TimeSpan(10, 15, 0)) return 2;
        if (time >= new TimeSpan(9, 15, 0)) return 1;
        return 0;
    }
}