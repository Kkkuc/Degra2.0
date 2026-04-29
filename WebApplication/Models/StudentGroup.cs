using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models;

public class StudentGroup
{
    public int StudentId { get; set; }
    [ForeignKey("StudentId")] public virtual Student Student { get; set; }
    public int GroupId { get; set; }
    [ForeignKey("GroupId")] public virtual Group Group { get; set; }
}