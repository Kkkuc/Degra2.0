namespace WebApplication.DTOs.Building;

public record BuildingAdminItemDto(
    int Id,
    string Name,
    int FacultyId,
    string FacultyName,
    string FacultyAbbreviation,
    string Street,
    string HouseNumber,
    string City,
    string PostalCode);
