using System.ComponentModel.DataAnnotations;

namespace WebApplication.DTOs.Student;

public sealed class StudentAdminFormDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Imię jest wymagane.")]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nazwisko jest wymagane.")]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Numer albumu jest wymagany.")]
    [StringLength(20)]
    public string StudentId { get; set; } = string.Empty;

    public List<int> GroupIds { get; set; } = [];
}