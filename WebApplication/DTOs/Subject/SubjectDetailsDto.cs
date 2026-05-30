namespace WebApplication.DTOs.Subject;

public record SubjectDetailsDto(
    int Id, 
    string Name, 
    string Abbreviation, 
    string Code);