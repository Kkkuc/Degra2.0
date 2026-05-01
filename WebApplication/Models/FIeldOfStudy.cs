using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models;

public class FieldOfStudy // ?????
{
    [Key] public int Id { get; set; }
    public int DepartmentId { get; set; }
    [ForeignKey("DepartmentId")] public virtual Faculty Faculty { get; set; }
    [Required, MaxLength(100)] public string Name { get; set; }
    [MaxLength(50)] public string Degree { get; set; } // e.g., Bachelor, Master
    [MaxLength(50)] public string Mode { get; set; } // e.g., Full-time, Part-time
    public virtual ICollection<Specialization> Specializations { get; set; }
}