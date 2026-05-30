using System.ComponentModel.DataAnnotations;

namespace WebApplication.DTOs.Student;

public class StudentFormDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Imię jest wymagane.")]
    [StringLength(50, ErrorMessage = "Imię nie może przekraczać 50 znaków.")]
    [Display(Name = "Imię")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nazwisko jest wymagane.")]
    [StringLength(50, ErrorMessage = "Nazwisko nie może przekraczać 50 znaków.")]
    [Display(Name = "Nazwisko")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Numer albumu jest wymagany.")]
    [StringLength(10, ErrorMessage = "Numer albumu nie może przekraczać 10 znaków.")]
    [Display(Name = "Numer albumu")]
    public string StudentId { get; set; } = string.Empty;
}