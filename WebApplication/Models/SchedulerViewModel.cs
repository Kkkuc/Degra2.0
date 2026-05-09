using WebApplication.Models;

namespace WebApplication.Models
{
    public class SchedulerViewModel
    {
        public List<TimetableEntryDto> Lessons { get; set; }
        public List<SubjectDto> Subjects { get; set; }
        public List<string> TimeSlots { get; set; }
    }

    public class TimetableEntryDto
    {
        public string Id { get; set; }
        public int SubjectId { get; set; }
        public string Subject { get; set; }
        public string Color { get; set; }
        public int Day { get; set; }
        public int StartSlot { get; set; }
        public int Duration { get; set; }
        public string Room { get; set; }
        public string Teacher { get; set; }
        public string Time { get; set; }
    }

    public class SubjectDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }
}