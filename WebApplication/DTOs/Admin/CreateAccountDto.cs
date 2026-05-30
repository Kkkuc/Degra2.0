using System.ComponentModel.DataAnnotations;

namespace WebApplication.DTOs.Admin;

public class CreateAccountDto
{
    [Required(ErrorMessage = "Nazwa użytkownika jest wymagana.")]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Adres e-mail jest wymagany.")]
    [EmailAddress(ErrorMessage = "Niepoprawny format adresu e-mail.")]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Hasło tymczasowe jest wymagane.")]
    [DataType(DataType.Password)]
    public string TempPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Wybór roli jest wymagany.")]
    public int RoleId { get; set; }
}