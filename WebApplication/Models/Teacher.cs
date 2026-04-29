using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models;

public class Teacher
{
    [Key] public int Id { get; set; }
    [MaxLength(50)] public string AcademicTitle { get; set; } // e.g., PhD, Prof.
    [Required, MaxLength(100)] public string FirstName { get; set; }
    [Required, MaxLength(100)] public string LastName { get; set; }
    [EmailAddress, MaxLength(100)] public string Email { get; set; }
}