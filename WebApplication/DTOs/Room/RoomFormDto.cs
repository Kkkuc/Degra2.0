using System.ComponentModel.DataAnnotations;
using WebApplication.Models.enums;

namespace WebApplication.DTOs.Room;

public class RoomFormDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Wybór budynku jest wymagany.")]
    [Display(Name = "Budynek")]
    public int BuildingId { get; set; }

    [Required(ErrorMessage = "Numer sali jest wymagany.")]
    [MaxLength(20, ErrorMessage = "Numer sali nie może przekraczać 20 znaków.")]
    [Display(Name = "Numer sali")]
    public string RoomNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Pojemność sali jest wymagana.")]
    [Range(1, 1000, ErrorMessage = "Pojemność musi mieścić się w przedziale od 1 do 1000.")]
    [Display(Name = "Pojemność")]
    public int? Capacity { get; set; }

    [Required(ErrorMessage = "Typ sali jest wymagany.")]
    [Display(Name = "Typ sali")]
    public RoomType RoomType { get; set; }
}