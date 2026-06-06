using System.ComponentModel.DataAnnotations;

namespace WebApplication.DTOs.Subject;

public class SubjectFormDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nazwa przedmiotu jest wymagana.")]
    [StringLength(200, ErrorMessage = "Nazwa nie może przekraczać 200 znaków.")]
    [Display(Name = "Nazwa przedmiotu")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Skrót przedmiotu jest wymagany.")]
    [StringLength(20, ErrorMessage = "Skrót nie może przekraczać 20 znaków.")]
    [Display(Name = "Skrót przedmiotu")]
    public string Abbreviation { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kod przedmiotu jest wymagany.")]
    [StringLength(50, ErrorMessage = "Kod nie może przekraczać 50 znaków.")]
    [Display(Name = "Kod przedmiotu")]
    public string Code { get; set; } = string.Empty;
}