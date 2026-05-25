using System.ComponentModel.DataAnnotations;

namespace WebApplication.DTOs;

public class AddressDto
{
    [Required(ErrorMessage = "Ulica jest wymagana")]
    public string Street { get; set; } = string.Empty;

    [Required(ErrorMessage = "Numer domu jest wymagany")]
    public string HouseNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Miasto jest wymagane")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kod pocztowy jest wymagany")]
    public string PostalCode { get; set; } = string.Empty;

}