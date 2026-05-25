// 3. DTO do formularzy (Create / Edit) - tutaj wykorzystujemy Twój AddressDto!

using WebApplication.DTOs;

public record BuildingDetailsDto(int Id, string Name, AddressDto AddressDto, int FacultyId, string FacultyName, string FacultyAbbreviation);