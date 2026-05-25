using System.ComponentModel.DataAnnotations;

namespace WebApplication.DTOs.Building;

public class BuildingFormDto
{
    public int Id { get; set; }
        
    [Required(ErrorMessage = "Nazwa budynku jest wymagana")]
    public string Name { get; set; } = string.Empty;
        
    [Required(ErrorMessage = "Wybór wydziału jest wymagany")]
    public int FacultyId { get; set; }
        
    public AddressDto AddressDto { get; set; } = new();
}