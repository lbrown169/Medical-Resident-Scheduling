using System.ComponentModel.DataAnnotations;
using MedicalDemo.Attributes;

namespace MedicalDemo.Enums;

public enum CallShiftType
{
    // Standard call shift types
    [CallShift(Hours = 3, CallLengthType = CallLengthType.Short, DateRule = CallShiftRule.Weekday)]
    [Display(Name = "Short")]
    WeekdayShortCall = 0,

    [CallShift(Hours = 24, CallLengthType = CallLengthType.Long, ApplicableDays = [DayOfWeek.Saturday], RequiredPgy = 1)]
    [Display(Name = "Saturday (24h)")]
    SaturdayFullCall = 1,

    [CallShift(Hours = 12, CallLengthType = CallLengthType.Long, ApplicableDays = [DayOfWeek.Saturday], RequiredPgy = 2)]
    [Display(Name = "Saturday (12h)")]
    SaturdayHalfCall = 2,

    [CallShift(Hours = 12, CallLengthType = CallLengthType.Long, ApplicableDays = [DayOfWeek.Sunday])]
    [Display(Name = "Sunday (12h)")]
    SundayHalfCall = 3,

    // First Saturday of July is training-specific, because PGY1s get their first week off
    [CallShift(Hours = 12, CallLengthType = CallLengthType.Long, RequiredPgy = 2, DateRule = CallShiftRule.FirstSaturdayOfJuly, Priority = 5)]
    [Display(Name = "Saturday (12h)")]
    FirstTrainingSaturdayHalfCall = 4,

    [CallShift(Hours = 12, CallLengthType = CallLengthType.Long, RequiredPgy = 2, DateRule = CallShiftRule.JulyFourth, Priority = 10)]
    [Display(Name = "July Fourth (12h)")]
    JulyFourth = 5,

    [CallShift(Hours = 12, CallLengthType = CallLengthType.Short, RequiredPgy = 2, DateRule = CallShiftRule.LaborDay, Priority = 10)]
    [Display(Name = "Labor Day (12h)")]
    LaborDay = 6,

    [CallShift(Hours = 24, CallLengthType = CallLengthType.Long, RequiredPgy = 1, DateRule = CallShiftRule.Thanksgiving, Priority = 10)]
    [Display(Name = "Thanksgiving (24h)")]
    Thanksgiving = 7,

    [CallShift(Hours = 24, CallLengthType = CallLengthType.Long, RequiredPgy = 1, DateRule = CallShiftRule.BlackFriday, Priority = 10)]
    [Display(Name = "Black Friday (24h)")]
    BlackFridayFullCall = 8,

    [CallShift(Hours = 12, CallLengthType = CallLengthType.Long, RequiredPgy = 2, DateRule = CallShiftRule.BlackFriday, Priority = 10)]
    [Display(Name = "Black Friday (12h)")]
    BlackFridayHalfCall = 9,

    [CallShift(Hours = 24, CallLengthType = CallLengthType.Long, RequiredPgy = 1, DateRule = CallShiftRule.ChristmasDay, Priority = 10)]
    [Display(Name = "Christmas Day (24h)")]
    ChristmasDay = 10,

    [CallShift(Hours = 24, CallLengthType = CallLengthType.Long, RequiredPgy = 1, DateRule = CallShiftRule.NewYearsDay, Priority = 10)]
    [Display(Name = "New Years Day (24h)")]
    NewYearsDay = 11,

    [CallShift(Hours = 12, CallLengthType = CallLengthType.Long, RequiredPgy = 1, DateRule = CallShiftRule.MemorialDay, Priority = 10)]
    [Display(Name = "Memorial Day (12h)")]
    MemorialDay = 12,

    [Display(Name = "Custom Shift")]
    Custom = 99
}