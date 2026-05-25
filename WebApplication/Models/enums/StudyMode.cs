using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models.enums;

public enum StudyMode
{
    [Display(Name = "Stacjonarne")]
    FullTime,

    [Display(Name = "Niestacjonarne")]
    PartTime,

    [Display(Name = "Podyplomowe")]
    Postgraduate
}