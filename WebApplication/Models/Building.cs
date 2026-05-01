using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models;

public class Building
{
    [Key] public int Id { get; set; }
    [Required, MaxLength(100)] public string Name { get; set; }
    [MaxLength(200)] public string Address { get; set; }
    [Required] public int FacultyId { get; set; }

    [ForeignKey(nameof(FacultyId))] public virtual Faculty Faculty { get; set; } = null!;

    public virtual ICollection<Room> Rooms { get; set; }
}