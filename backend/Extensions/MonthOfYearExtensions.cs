using MedicalDemo.Enums;

namespace MedicalDemo.Extensions;

public static class MonthOfYearExtensions
{
    public static int ToCalendarIndex(this MonthOfYear month) => (int)month;
    public static int ToAcademicIndex(this MonthOfYear month) => (((int)month) + 6) % 12;

    public static MonthOfYear FromCalendarIndex(int index, bool wrap) => (MonthOfYear)
    (wrap
        ? index % 12
        : index > 11
            ? throw new ArgumentException("Invalid wrap")
            : index
    );
    public static MonthOfYear FromAcademicIndex(int index, bool wrap) => (MonthOfYear)
        (((wrap
            ? index % 12
            : index > 11
                ? throw new ArgumentException("Invalid wrap")
                : index
        ) + 6) % 12);

    // DateTime/DateOnly.Month is 1-indexed
    public static MonthOfYear FromDateTime(DateTime datetime, bool wrap) => FromCalendarIndex(datetime.Month - 1, wrap);
    public static MonthOfYear FromDateOnly(DateOnly dateOnly, bool wrap) => FromCalendarIndex(dateOnly.Month - 1, wrap);

    public static MonthOfYear Next(this MonthOfYear month) => FromCalendarIndex(month.ToCalendarIndex() + 1, true);
    public static MonthOfYear Previous(this MonthOfYear month) => FromCalendarIndex(month.ToCalendarIndex() - 1, true);
}