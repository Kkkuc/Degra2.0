using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models;

public class User
{
    [Key] public int Id { get; set; }
    [Required, MaxLength(50)] public string Username { get; set; }
    [Required, MaxLength(255)] public string PasswordHash { get; set; }

    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; }

    public int RoleId { get; set; }
    [ForeignKey("RoleId")] public virtual Role Role { get; set; }
    public int? StudentId { get; set; }
    [ForeignKey("StudentId")] public virtual Student Student { get; set; }
    public int? TeacherId { get; set; }
    [ForeignKey("TeacherId")] public virtual Teacher Teacher { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLogin { get; set; }
}