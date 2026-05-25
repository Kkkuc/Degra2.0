using System.ComponentModel.DataAnnotations;
using WebApplication.Models.enums;

namespace WebApplication.DTOs.Group;

public class GroupFormDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Wybor semestru jest wymagany")]
    [Display(Name = "Semestr")]
    public int SemesterId { get; set; }

    [Required(ErrorMessage = "Wybor kierunku studiow jest wymagany")]
    [Display(Name = "Kierunek studiow")]
    public int FieldOfStudyId { get; set; }

    [Display(Name = "Specjalizacja")]
    public int? SpecializationId { get; set; }

    [Required(ErrorMessage = "Typ zajec jest wymagany")]
    [Display(Name = "Typ zajec")]
    public ClassType ClassType { get; set; }

    [Required(ErrorMessage = "Nazwa grupy jest wymagana")]
    [MaxLength(50, ErrorMessage = "Nazwa nie moze przekraczac 50 znakow")]
    [Display(Name = "Nazwa grupy")]
    public string Name { get; set; } = string.Empty;
}
