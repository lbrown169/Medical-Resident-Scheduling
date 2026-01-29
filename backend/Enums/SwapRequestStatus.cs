using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Enums;

public enum SwapRequestStatus
{
    [Display(Name = "Pending")]
    Pending = 0,
    [Display(Name = "Approved")]
    Approved = 1,
    [Display(Name = "Denied")]
    Denied = 2
}