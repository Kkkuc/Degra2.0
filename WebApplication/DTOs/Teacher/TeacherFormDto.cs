using System.ComponentModel.DataAnnotations;

namespace WebApplication.DTOs.Teacher;

public sealed class TeacherFormDto
{
    public int Id { get; set; }

    [StringLength(
        50,
        ErrorMessage = "Tytuł naukowy nie może przekraczać 50 znaków.")]
    public string? AcademicTitle { get; set; }

    [Required(ErrorMessage = "Imię jest wymagane.")]
    [StringLength(
        100,
        ErrorMessage = "Imię nie może przekraczać 100 znaków.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nazwisko jest wymagane.")]
    [StringLength(
        100,
        ErrorMessage = "Nazwisko nie może przekraczać 100 znaków.")]
    public string LastName { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Podaj prawidłowy adres e-mail.")]
    [StringLength(
        200,
        ErrorMessage = "Adres e-mail nie może przekraczać 200 znaków.")]
    public string? Email { get; set; }
}