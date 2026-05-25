using System.ComponentModel.DataAnnotations;

namespace WebApplication.DTOs;

public class DateRangeDto
{
    [Required(ErrorMessage = "Data rozpoczęcia jest wymagana")]
    [DataType(DataType.Date)]
    [Display(Name = "Data rozpoczęcia")]
    public DateOnly StartDate { get; set; }

    [Required(ErrorMessage = "Data zakończenia jest wymagana")]
    [DataType(DataType.Date)]
    [Display(Name = "Data zakończenia")]
    public DateOnly EndDate { get; set; }
}