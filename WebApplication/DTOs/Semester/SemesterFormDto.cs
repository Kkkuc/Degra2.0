using System.ComponentModel.DataAnnotations;

namespace WebApplication.DTOs.Semester;

public class SemesterFormDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Wybór roku akademickiego jest wymagany.")]
    [Display(Name = "Rok akademicki")]
    public int AcademicYearId { get; set; }

    [Required(ErrorMessage = "Nazwa semestru jest wymagana.")]
    [StringLength(20, ErrorMessage = "Nazwa nie może przekraczać 20 znaków.")]
    [Display(Name = "Nazwa semestru")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Data rozpoczęcia jest wymagana.")]
    [DataType(DataType.Date)]
    [Display(Name = "Data rozpoczęcia")]
    public DateOnly StartDate { get; set; }

    [Required(ErrorMessage = "Data zakończenia jest wymagana.")]
    [DataType(DataType.Date)]
    [Display(Name = "Data zakończenia")]
    public DateOnly EndDate { get; set; }
}