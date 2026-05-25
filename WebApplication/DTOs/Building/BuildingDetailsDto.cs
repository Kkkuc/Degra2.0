namespace WebApplication.DTOs.Building;

public record BuildingDetailsDto(int Id, string Name, AddressDto AddressDto, int FacultyId, string FacultyName, string FacultyAbbreviation);