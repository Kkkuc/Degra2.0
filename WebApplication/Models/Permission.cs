using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models;

public class Permission
{
    [Key] public int Id { get; set; }
    [Required, MaxLength(100)] public string PermissionCode { get; set; } // e.g., "EDIT_OWN_SCHEDULE"
    public string Description { get; set; }
    public virtual ICollection<RolePermission> RolePermissions { get; set; }
}