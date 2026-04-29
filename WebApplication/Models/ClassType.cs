using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models;

public class ClassType //Use somewhere enum change it
{
    [Key] public int Id { get; set; }
    [Required, MaxLength(100)] public string Name { get; set; } // e.g., Laboratory, Lecture
    [Required, MaxLength(10)] public string Abbreviation { get; set; } // e.g., LAB, LEC
}