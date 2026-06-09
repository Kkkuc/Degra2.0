using System.ComponentModel.DataAnnotations;

namespace WebApplication.DTOs.Room;

public sealed class RoomFormDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Numer sali jest wymagany.")]
    [StringLength(50)]
    public string RoomNumber { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Wybierz budynek.")]
    public int BuildingId { get; set; }
}