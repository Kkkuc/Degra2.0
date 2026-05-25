using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication.Models.enums;

namespace WebApplication.Models;

public class FieldOfStudy 
{
    [Key] public int Id { get; set; }
    public int FacultyId { get; set; }
    [ForeignKey("FacultyId")] public virtual Faculty? Faculty { get; set; }
    [Required, MaxLength(100)] public string Name { get; set; }
    [MaxLength(50)] public string Degree { get; set; } // e.g., Bachelor, Master
    public StudyMode Mode { get; set; }
    public ICollection<Group>? Groups { get; set; }
}