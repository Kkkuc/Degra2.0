using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Models.enums; // Ensure this matches your project structure

namespace WebApplication.Controllers;

public class SchedulerController : Controller
{
    private readonly AppDbContext _context;

    public SchedulerController(AppDbContext context) => _context = context;

    public async Task<IActionResult> Index(int? fieldId, int? semId, string gW, string gC, string gPs, string gP, string gL, string gJ, string gS, string gWf)
    {
        // Populate dropdowns for the form
        ViewBag.Fields = await _context.FieldsOfStudy.ToListAsync();
        ViewBag.Semesters = await _context.Semesters.ToListAsync();

        var query = _context.Timetables
            .Include(t => t.Subject)
            .Include(t => t.Teacher)
            .Include(t => t.Room)
            .Include(t => t.Group)
            .AsQueryable();

        // 1. Contextual Filtering (Field of Study & Semester)
        // This prevents showing classes from other majors/years
        if (fieldId.HasValue) query = query.Where(t => t.Group.FieldOfStudyId == fieldId);
        if (semId.HasValue) query = query.Where(t => t.Group.SemesterId == semId);

        // 2. Strict Class-Type Filtering
        // Using 'OR' between types, but 'AND' within each type (Group Name + Type)
        var hasGroupFilters = !string.IsNullOrEmpty(gW) || !string.IsNullOrEmpty(gC) || !string.IsNullOrEmpty(gL) || 
                              !string.IsNullOrEmpty(gPs) || !string.IsNullOrEmpty(gP) || !string.IsNullOrEmpty(gS);

        if (hasGroupFilters)
        {
            query = query.Where(t => 
                (!string.IsNullOrEmpty(gW) && t.Group.ClassType == ClassType.Lecture && t.Group.Name == "grupa " + gW) ||
                (!string.IsNullOrEmpty(gC) && t.Group.ClassType == ClassType.Exercise && t.Group.Name == "grupa " + gC) ||
                (!string.IsNullOrEmpty(gL) && t.Group.ClassType == ClassType.Laboratory && t.Group.Name == "grupa " + gL) ||
                (!string.IsNullOrEmpty(gPs) && t.Group.ClassType == ClassType.SpecialisedLaboratory && t.Group.Name == "grupa " + gPs) ||
                (!string.IsNullOrEmpty(gP) && t.Group.ClassType == ClassType.Project && t.Group.Name == "grupa " + gP) ||
                (!string.IsNullOrEmpty(gS) && t.Group.ClassType == ClassType.Seminar && t.Group.Name == "grupa " + gS)
            );
        }

        var timetables = await query.ToListAsync();
        var subjects = await _context.Subjects.ToListAsync();

        var viewModel = new SchedulerViewModel
        {
            Lessons = timetables.Select(t => new TimetableEntryDto
            {
                Id = t.Id.ToString(),
                SubjectId = t.SubjectId,
                Subject = t.Subject.Name,
                Color = GetColorForSubject(t.Subject.Id),
                Day = (int)t.DayOfWeek - 1,
                StartSlot = GetSlotIndex(t.StartTime),
                // Height based on actual duration (45-min slots)
                Duration = (int)((t.EndTime - t.StartTime).TotalMinutes / 45),
                Room = t.Room.RoomNumber,
                Teacher = $"{t.Teacher.FirstName} {t.Teacher.LastName}",
                Time = $"{t.StartTime:hh\\:mm} – {t.EndTime:hh\\:mm}"
            }).ToList(),
            Subjects = subjects.Select(s => new SubjectDto { Id = s.Id.ToString(), Name = s.Name, Color = GetColorForSubject(s.Id) }).ToList(),
            TimeSlots = new List<string> { "08:30 – 09:15", "09:15 – 10:00", "10:15 – 11:00", "11:00 – 11:45", "12:00 – 12:45", "12:45 – 13:30", "14:00 – 14:45", "14:45 – 15:30", "16:00 – 16:45", "16:45 – 17:30", "17:40 – 18:25", "18:25 – 19:10" }
        };

        return View(viewModel);
    }

    private string GetColorForSubject(int id) => (new[] { "#EF4444", "#F97316", "#22C55E", "#3B82F6", "#8B5CF6", "#EC4899" })[id % 6];

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