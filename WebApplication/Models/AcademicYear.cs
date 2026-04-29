using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models;

public class AcademicYear
{
    [Key] public int Id { get; set; }
    [Required, MaxLength(20)] public string Name { get; set; } // e.g., "2025/2026"
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public virtual ICollection<Semester> Semesters { get; set; } 
}