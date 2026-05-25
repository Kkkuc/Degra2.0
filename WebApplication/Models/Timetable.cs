using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication.Models.enums;

namespace WebApplication.Models;

public class Timetable
{
    [Key] public int Id { get; set; }

    [NotMapped]
    public string FullDisplayInfo =>
        $"{Subject?.Name} ({Teacher?.LastName}) - {DayOfWeek}, {StartTime:hh\\:mm}";

    public int SubjectId { get; set; }
    [ForeignKey("SubjectId")] public virtual Subject? Subject { get; set; }
    public int TeacherId { get; set; }
    [ForeignKey("TeacherId")] public virtual Teacher? Teacher { get; set; }
    public int RoomId { get; set; }
    [ForeignKey("RoomId")] public virtual Room? Room { get; set; }
    public int GroupId { get; set; }
    [ForeignKey("GroupId")] public virtual Group? Group { get; set; }
    [Required] public ClassType ClassType { get; set; } // Zmienione z int na Enum
    [Required] public DayOfWeek DayOfWeek { get; set; }
    [DataType(DataType.Time)] public TimeSpan StartTime { get; set; }
    [DataType(DataType.Time)] public TimeSpan EndTime { get; set; }
    [Required] public WeekCycle WeekCycle { get; set; }
}