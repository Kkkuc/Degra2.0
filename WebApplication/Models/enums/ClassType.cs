using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models.enums;

public enum ClassType
{
    [Display(Name = "Wykład")] Lecture,
    [Display(Name = "Laboratorium")] Laboratory,
    [Display(Name = "Pracownia specjalistyczna")] SpecialisedLaboratory,
    [Display(Name = "Ćwiczenia")] Exercise,
    [Display(Name = "Seminarium")] Seminar,
    [Display(Name = "Projekt")] Project
}