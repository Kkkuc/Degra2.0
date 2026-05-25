using System.ComponentModel.DataAnnotations;

namespace WebApplication.DTOs;

public class AcademicYearFormDto
{
    public int Id { get; set; }
        
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;
        
    [Required]
    public DateOnly StartDate { get; set; }
        
    [Required]
    public DateOnly EndDate { get; set; }
}