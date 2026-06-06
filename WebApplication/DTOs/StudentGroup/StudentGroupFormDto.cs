using System.ComponentModel.DataAnnotations;

namespace WebApplication.DTOs.StudentGroup;

public class StudentGroupFormDto
{
    [Required(ErrorMessage = "Wybór studenta jest wymagany.")]
    [Display(Name = "Student")]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "Wybór grupy jest wymagana.")]
    [Display(Name = "Grupa")]
    public int GroupId { get; set; }
}