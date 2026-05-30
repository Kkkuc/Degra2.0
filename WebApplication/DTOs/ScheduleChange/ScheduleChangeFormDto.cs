using System.ComponentModel.DataAnnotations;

namespace WebApplication.DTOs.ScheduleChange;

public class ScheduleChangeFormDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Wybór zajęć z planu jest wymagany.")]
    [Display(Name = "Oryginalny wpis w planie")]
    public int TimetableId { get; set; }

    [Required(ErrorMessage = "Data zmiany jest wymagana.")]
    [DataType(DataType.Date)]
    [Display(Name = "Data zmiany")]
    public DateTime? ChangeDate { get; set; }

    [Display(Name = "Nowa sala")]
    public int? NewRoomId { get; set; }

    [Display(Name = "Nowy prowadzący")]
    public int? NewTeacherId { get; set; }

    [Display(Name = "Nowa godzina rozpoczęcia")]
    [DataType(DataType.Time)]
    public TimeSpan? NewStartTime { get; set; }

    [Display(Name = "Nowa godzina zakończenia")]
    [DataType(DataType.Time)]
    public TimeSpan? NewEndTime { get; set; }
}