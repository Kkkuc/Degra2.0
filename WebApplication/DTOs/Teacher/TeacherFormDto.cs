using System.ComponentModel.DataAnnotations;

namespace WebApplication.DTOs.Teacher;

public class TeacherFormDto
{
    public int Id { get; set; }

    [StringLength(30, ErrorMessage = "Tytuł/stopień naukowy nie może przekraczać 30 znaków.")]
    [Display(Name = "Tytuł naukowy")]
    public string AcademicTitle { get; set; } = string.Empty;

    [Required(ErrorMessage = "Imię jest wymagane.")]
    [StringLength(50, ErrorMessage = "Imię nie może przekraczać 50 znaków.")]
    [Display(Name = "Imię")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nazwisko jest wymagane.")]
    [StringLength(50, ErrorMessage = "Nazwisko nie może przekraczać 50 znaków.")]
    [Display(Name = "Nazwisko")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Adres e-mail jest wymagany.")]
    [EmailAddress(ErrorMessage = "Niepoprawny format adresu e-mail.")]
    [StringLength(100, ErrorMessage = "Adres e-mail nie może przekraczać 100 znaków.")]
    [Display(Name = "Adres e-mail")]
    public string Email { get; set; } = string.Empty;
}