namespace WebApplication.DTOs.FieldOfStudy;

public class FieldOfStudyIndexDto(int id, string name, string degree, int mode, string facultyAbbreviation)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string Degree { get; set; } = degree; // Dodaj to
    public int Mode { get; set; } = mode; // Dodaj to (jako int)
    public string FacultyAbbreviation { get; set; } = facultyAbbreviation; // Opcjonalnie
}