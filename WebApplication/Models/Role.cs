using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models;

public class Role
{
    [Key] public int Id { get; set; }
    [Required, MaxLength(50)] public string Name { get; set; }
    public virtual ICollection<RolePermission>? RolePermissions { get; set; }
}