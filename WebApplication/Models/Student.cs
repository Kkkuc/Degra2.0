using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models;

public class Student
{
    [Key] public int Id { get; set; }
    [Required, MaxLength(20)] public string StudentID { get; set; } // Index number
    [Required, MaxLength(100)] public string FirstName { get; set; }
    [Required, MaxLength(100)] public string LastName { get; set; }
    public virtual ICollection<StudentGroup>? StudentGroups { get; set; }
}