using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models;

public class Building
{
    [Key] public int Id { get; set; }
    [Required, MaxLength(100)] public string Name { get; set; }
    [Required, MaxLength(150)] public string Street { get; set; }

    [Required, MaxLength(10)] public string HouseNumber { get; set; }

    [Required, MaxLength(100)] public string City { get; set; }

    [Required, MaxLength(10)]
    [RegularExpression(@"\d{2}-\d{3}", ErrorMessage = "Format kodu pocztowego to 00-000")]
    public string PostalCode { get; set; }

    [Required] public int FacultyId { get; set; }

    [ForeignKey(nameof(FacultyId))] public virtual Faculty? Faculty { get; set; }

    public virtual ICollection<Room>? Rooms { get; set; }
}