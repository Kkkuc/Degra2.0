namespace WebApplication.DTOs.Teacher;

public class TeacherDropdownDto
{
    public int Id { get; set; }
    public string AcademicTitle { get; set; } 
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public string FullDisplayName => 
        string.IsNullOrWhiteSpace(AcademicTitle) 
            ? $"{FirstName[..1]} {LastName}" 
            : $"{AcademicTitle} {FirstName[..1]} {LastName}";
}