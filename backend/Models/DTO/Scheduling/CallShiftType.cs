using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Models.DTO.Scheduling;

public enum CallShiftType
{
    [Display(Name = "Short")]
    WeekdayShortCall = 0,
    [Display(Name = "Saturday (24h)")]
    SaturdayFullCall = 1,
    [Display(Name = "Saturday (14h)")]
    SaturdayHalfCall = 2,
    [Display(Name = "Sunday (12h)")]
    SundayHalfCall = 3
}