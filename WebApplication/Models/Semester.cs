using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models;

public class Semester
{
    [Key] public int Id { get; set; }
    public int AcademicYearId { get; set; }
    [ForeignKey("AcademicYearId")] public virtual AcademicYear? AcademicYear { get; set; }
    [Required, MaxLength(20)] public string Name { get; set; } // e.g., "Winter", "Summer"
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public virtual ICollection<Group>? Groups { get; set; }
}