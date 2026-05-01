using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication.Models.enums;

namespace WebApplication.Models;

public class Group
{
    [Key] public int Id { get; set; }
    public int SemesterId { get; set; }
    [ForeignKey("SemesterId")] public virtual Semester Semester { get; set; }
    public int SpecializationId { get; set; }

    [ForeignKey("SpecializationId")]
    public virtual Specialization Specialization { get; set; } // It needs to be changed

    [Required] public ClassType ClassType { get; set; } // Zmienione z int na Enum
    [Required, MaxLength(50)] public string Name { get; set; } // e.g., "Group L5"
    public virtual ICollection<StudentGroup> StudentGroups { get; set; }
}