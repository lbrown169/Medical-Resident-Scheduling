namespace MedicalDemo.Algorithm;

public enum CallShiftType
{
    WeekdayShortCall = 0,
    SaturdayFullCall = 1,
    SaturdayHalfCall = 2,
    SundayHalfCall = 3
}

public static class CallShiftTypeExtensions
{
    public static List<CallShiftType> GetAllCallShiftTypes()
    {
        return Enum.GetValues<CallShiftType>().ToList();
    }

    public static List<CallShiftType> GetAllCallShiftTypesForDate(DateTime date)
    {
        switch (date.DayOfWeek)
        {
            case DayOfWeek.Monday:
            case DayOfWeek.Tuesday:
            case DayOfWeek.Wednesday:
            case DayOfWeek.Thursday:
            case DayOfWeek.Friday:
                return [CallShiftType.WeekdayShortCall];
            case DayOfWeek.Saturday:
                return [CallShiftType.SaturdayFullCall, CallShiftType.SaturdayHalfCall];
            case DayOfWeek.Sunday:
                return [CallShiftType.SundayHalfCall];
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static CallShiftType GetCallShiftTypeForDate(DateTime date, int year)
    {
        switch (date.DayOfWeek)
        {
            case DayOfWeek.Monday:
            case DayOfWeek.Tuesday:
            case DayOfWeek.Wednesday:
            case DayOfWeek.Thursday:
            case DayOfWeek.Friday:
                return CallShiftType.WeekdayShortCall;
            case DayOfWeek.Saturday when year == 1:
                return CallShiftType.SaturdayFullCall;
            case DayOfWeek.Saturday when year == 2:
                return CallShiftType.SaturdayHalfCall;
            case DayOfWeek.Sunday:
                return CallShiftType.SundayHalfCall;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static List<CallShiftType> GetAllCallShiftsForYear(int year)
    {
        List<CallShiftType> shifts = [CallShiftType.WeekdayShortCall, CallShiftType.SundayHalfCall];

        switch (year)
        {
            case 1:
                shifts.Add(CallShiftType.SaturdayFullCall);
                break;
            case 2:
                shifts.Add(CallShiftType.SaturdayHalfCall);
                break;
        }

        return shifts;
    }

    public static int? GetRequiredYear(this CallShiftType callShiftType)
    {
        switch (callShiftType)
        {
            case CallShiftType.WeekdayShortCall:
            case CallShiftType.SundayHalfCall:
                return null;
            case CallShiftType.SaturdayFullCall:
                return 1;
            case CallShiftType.SaturdayHalfCall:
                return 2;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static int GetHours(this CallShiftType callShiftType)
    {
        switch (callShiftType)
        {
            case CallShiftType.WeekdayShortCall:
                return 3;
            case CallShiftType.SundayHalfCall:
                return 12;
            case CallShiftType.SaturdayFullCall:
                return 24;
            case CallShiftType.SaturdayHalfCall:
                return 12;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static string GetDescription(this CallShiftType callShiftType)
    {
        switch (callShiftType)
        {
            case CallShiftType.WeekdayShortCall:
                return "Short";
            case CallShiftType.SaturdayHalfCall:
                return "Saturday (12h)";
            case CallShiftType.SaturdayFullCall:
                return "Saturday (24h)";
            case CallShiftType.SundayHalfCall:
                return "Sunday (12h)";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}