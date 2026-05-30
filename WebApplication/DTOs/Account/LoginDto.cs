using System.ComponentModel.DataAnnotations;

namespace WebApplication.DTOs.Account;

public class LoginDto
{
    [Required(ErrorMessage = "Nazwa użytkownika jest wymagana.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Hasło jest wymagane.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}