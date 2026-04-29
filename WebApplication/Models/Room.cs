using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models;

public class Room
{
    [Key] public int Id { get; set; }
    public int BuildingId { get; set; }
    [ForeignKey("BuildingId")] public virtual Building Building { get; set; }
    [Required, MaxLength(20)] public string Number { get; set; }
    public int? Capacity { get; set; }
    [MaxLength(50)] public string Type { get; set; } // e.g., Laboratory, Lecture Hall
}