namespace WebApplication.DTOs.StudentGroup;

public record StudentGroupDto(
    int StudentId, 
    string StudentFullName, 
    int GroupId, 
    string GroupName);