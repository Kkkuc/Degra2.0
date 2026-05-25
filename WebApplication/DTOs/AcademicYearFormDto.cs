using System.ComponentModel.DataAnnotations;

namespace WebApplication.DTOs;

public class AcademicYearFormDto
{
    public int Id { get; set; } // Dla edycji, przy tworzeniu będzie 0
        
    [Required(ErrorMessage = "Nazwa jest wymagana")]
    public string Name { get; set; } = string.Empty;
        
    [Required]
    public DateOnly StartDate { get; set; }
        
    [Required]
    public DateOnly EndDate { get; set; }
}