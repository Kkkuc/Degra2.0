using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models.enums;

public enum WeekCycle
{
    [Display(Name = "Co tydzień")] Weekly,
    [Display(Name = "Tydzień parzysty")] Even,

    [Display(Name = "Tydzień nieparzysty")]
    Odd
}