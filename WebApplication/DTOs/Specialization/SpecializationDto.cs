using System.ComponentModel.DataAnnotations;

namespace WebApplication.DTOs.Specialization;

public record SpecializationDto(
    int Id,
    
    [Required(ErrorMessage = "Nazwa specjalizacji jest wymagana.")]
    [StringLength(100, ErrorMessage = "Nazwa nie może przekraczać 100 znaków.")]
    [Display(Name = "Nazwa specjalizacji")]
    string Name
);