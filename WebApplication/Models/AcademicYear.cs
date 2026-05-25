using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models;

public class AcademicYear
{
    [Key] public int Id { get; set; }

    [Required(ErrorMessage = "Year name is required"), MaxLength(20), Display(Name = "Year name")]
    public string Name { get; set; } // e.g., "2025/2026"

    [DataType(DataType.Date)] public DateOnly StartDate { get; set; }
    [DataType(DataType.Date)] public DateOnly EndDate { get; set; }
    public virtual ICollection<Semester>? Semesters { get; set; }
}