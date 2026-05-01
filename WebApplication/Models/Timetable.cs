using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication.Models.enums;

namespace WebApplication.Models;

public class Timetable
{
    [Key] public int Id { get; set; }
    public int SubjectId { get; set; }
    [ForeignKey("SubjectId")] public virtual Subject Subject { get; set; }
    public int TeacherId { get; set; }
    [ForeignKey("TeacherId")] public virtual Teacher Teacher { get; set; }
    public int RoomId { get; set; }
    [ForeignKey("RoomId")] public virtual Room Room { get; set; }
    public int GroupId { get; set; }
    [ForeignKey("GroupId")] public virtual Group Group { get; set; }
    //public int ClassTypeId { get; set; }
    //[ForeignKey("ClassTypeId")] public virtual ClassType ClassType { get; set; }
    [Range(1, 7)] public int DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    [Required] public WeekCycle WeekCycle { get; set; }
}