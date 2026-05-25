using System.ComponentModel.DataAnnotations;

namespace WebApplication.DTOs.AcademicYear;

public class AcademicYearFormDto
{
    public int Id { get; set; } 
    
    public DateRangeDto Period { get; set; } = new();
}