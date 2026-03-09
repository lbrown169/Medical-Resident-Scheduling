using MedicalDemo.Algorithms.OnCallScheduleGenerator;
using MedicalDemo.Enums;

namespace MedicalDemo.Extensions;

public static class CallShiftRuleExtensions
{
    public static bool Applies(this CallShiftRule rule, DateOnly date) => rule switch
    {
        CallShiftRule.None => true,
        CallShiftRule.Weekday => date.DayOfWeek is >= DayOfWeek.Monday and <= DayOfWeek.Friday,
        CallShiftRule.FirstSaturdayOfJuly => IsNthWeekdayOfMonth(date, DayOfWeek.Saturday, 7, 1),
        CallShiftRule.JulyFourth => date is { Month: 7, Day: 4 },
        CallShiftRule.LaborDay => IsNthWeekdayOfMonth(date, DayOfWeek.Monday, 9, 1),
        CallShiftRule.Thanksgiving => IsNthWeekdayOfMonth(date, DayOfWeek.Thursday, 11, 4),
        CallShiftRule.BlackFriday => CallShiftRule.Thanksgiving.Applies(date.AddDays(-1)),
        CallShiftRule.ChristmasDay => date is { Month: 12, Day: 25 },
        CallShiftRule.NewYearsDay => date is { Month: 1, Day: 1 },
        CallShiftRule.MemorialDay => IsLastWeekdayOfMonth(date, DayOfWeek.Monday, 5),
        _ => throw new ArgumentOutOfRangeException(nameof(rule), rule, null)
    };

    private static bool IsNthWeekdayOfMonth(DateOnly date, DayOfWeek dow, int month, int nth)
    {
        if (date.Month != month || date.DayOfWeek != dow)
        {
            return false;
        }

        return (date.Day - 1) / 7 + 1 == nth;
    }

    private static bool IsLastWeekdayOfMonth(DateOnly date, DayOfWeek dow, int month)
    {
        if (date.Month != month || date.DayOfWeek != dow)
        {
            return false;
        }

        // If adding 7 days pushes us into the next month, this is the last one
        return date.AddDays(7).Month != date.Month;
    }
}