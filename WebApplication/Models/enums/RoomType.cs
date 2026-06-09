using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models.enums;

public enum RoomType
{
    [Display(Name = "Sala wykładowa")] LectureHall,
    [Display(Name = "Laboratorium")] Laboratory,
    [Display(Name = "Sala ćwiczeniowa")] SeminarRoom,

    [Display(Name = "Pracownia komputerowa")]
    ComputerLab,
    [Display(Name = "Inne")] Other
}