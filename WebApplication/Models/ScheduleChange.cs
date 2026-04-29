using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models;

public class ScheduleChange
{
    [Key] public int Id { get; set; }
    public int TimetableId { get; set; }
    [ForeignKey("TimetableId")] public virtual Timetable OriginalEntry { get; set; }
    public DateTime ChangeDate { get; set; }
    [Required, MaxLength(50)] public string ChangeType { get; set; } // e.g., Cancellation, Relocation
    public int? NewRoomId { get; set; }
    [ForeignKey("NewRoomId")] public virtual Room NewRoom { get; set; }
    public int? NewTeacherId { get; set; }
    [ForeignKey("NewTeacherId")] public virtual Teacher NewTeacher { get; set; }
    public TimeSpan? NewStartTime { get; set; }
    public TimeSpan? NewEndTime { get; set; }
}