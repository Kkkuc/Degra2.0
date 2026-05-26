using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication.Models.enums;

namespace WebApplication.Models;

public class Room
{
    [Key] public int Id { get; set; }
    public int BuildingId { get; set; }
    [ForeignKey("BuildingId")] public virtual Building? Building { get; set; }
    [Required, MaxLength(20)] public string RoomNumber { get; set; }
    public int? Capacity { get; set; }
    [Required] public RoomType RoomType { get; set; }
}