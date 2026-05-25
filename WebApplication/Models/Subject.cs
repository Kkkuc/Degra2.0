using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models;

public class Subject
{
    [Key] public int Id { get; set; }
    [Required, MaxLength(200)] public string Name { get; set; }
    [MaxLength(20)] public string Abbreviation { get; set; }
    [MaxLength(50)] public string Code { get; set; }
}