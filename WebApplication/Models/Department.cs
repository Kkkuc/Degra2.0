using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models;

public class Department
{
    [Key] public int Id { get; set; }
    [Required, MaxLength(100)] public string Name { get; set; }
    [MaxLength(10)] public string Abbreviation { get; set; }
    public virtual ICollection<FieldOfStudy> FieldsOfStudy { get; set; } /// ????
}