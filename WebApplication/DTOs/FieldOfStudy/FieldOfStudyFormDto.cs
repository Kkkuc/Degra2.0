using System.ComponentModel.DataAnnotations;
using WebApplication.Models.enums;

namespace WebApplication.DTOs.FieldOfStudy;

public class FieldOfStudyFormDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nazwa kierunku studiów jest wymagana")]
    [MaxLength(100, ErrorMessage = "Nazwa nie może przekraczać 100 znaków")]
    [Display(Name = "Nazwa kierunku")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Stopień studiów jest wymagany")]
    [MaxLength(50, ErrorMessage = "Stopień nie może przekraczać 50 znaków")]
    [Display(Name = "Stopień studiów")]
    public string Degree { get; set; } = string.Empty;

    [Required(ErrorMessage = "Wybór wydziału jest wymagany")]
    [Display(Name = "Wydział")]
    public int FacultyId { get; set; }

    [Required(ErrorMessage = "Wybór trybu studiów jest wymagany")]
    [Display(Name = "Tryb studiów")]
    public StudyMode Mode { get; set; }
}