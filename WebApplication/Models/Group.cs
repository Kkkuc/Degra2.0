using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models;

public class Group
{
    [Key] public int Id { get; set; }
    public int SemesterId { get; set; }
    [ForeignKey("SemesterId")] public virtual Semester Semester { get; set; }
    public int SpecializationId { get; set; }
    [ForeignKey("SpecializationId")] public virtual Specialization Specialization { get; set; }
    public int ClassTypeId { get; set; }
    [ForeignKey("ClassTypeId")] public virtual ClassType ClassType { get; set; }
    [Required, MaxLength(50)] public string Name { get; set; } // e.g., "Group L5"
    public virtual ICollection<StudentGroup> StudentGroups { get; set; }
}