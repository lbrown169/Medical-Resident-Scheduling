using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Enums;

public enum ChiefType
{
    None = 0,

    [Display(Name = "Admin")]
    Admin = 1,

    [Display(Name = "Clinic")]
    Clinic = 2,

    [Display(Name = "Education")]
    Education = 3,
}