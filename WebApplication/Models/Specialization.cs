using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models;

public class Specialization
{
    [Key] public int Id { get; set; }
    public int FieldOfStudyId { get; set; }
    [ForeignKey("FieldOfStudyId")] public virtual FieldOfStudy FieldOfStudy { get; set; }
    [Required, MaxLength(100)] public string Name { get; set; }
    public virtual ICollection<Group> Groups { get; set; }
}