using System.ComponentModel.DataAnnotations;
using WebApplication.Models.enums;

namespace WebApplication.DTOs.Timetable;

public class TimetableFilterDto
{
    [Display(Name = "Przedmiot")]
    public int? SubjectId { get; set; }

    [Display(Name = "Prowadzący")]
    public int? TeacherId { get; set; }

    [Display(Name = "Sala")]
    public int? RoomId { get; set; }

    [Display(Name = "Grupa")]
    public int? GroupId { get; set; }

    [Display(Name = "Typ zajęć")]
    public ClassType? ClassType { get; set; }

    [Display(Name = "Dzień")]
    public DayOfWeek? DayOfWeek { get; set; }

    [Display(Name = "Cykl")]
    public WeekCycle? WeekCycle { get; set; }
}