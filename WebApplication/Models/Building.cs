using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models;

public class Building
{
    [Key] public int Id { get; set; }
    [Required, MaxLength(100)] public string Name { get; set; }
    [MaxLength(200)] public string Address { get; set; }
    public virtual ICollection<Room> Rooms { get; set; }
}