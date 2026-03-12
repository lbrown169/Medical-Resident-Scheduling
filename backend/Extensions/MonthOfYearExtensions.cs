using MedicalDemo.Enums;

namespace MedicalDemo.Extensions;

public static class MonthOfYearExtensions
{
    public static int ToCalendarIndex(this MonthOfYear month) => (int)month;
    public static int ToAcademicIndex(this MonthOfYear month) => (((int)month) + 6) % 12;

    public static MonthOfYear FromCalendarIndex(int index, bool wrap) => (MonthOfYear)(wrap ? index % 12 : index); // add boundary exceptions if wrap is false
    public static MonthOfYear FromAcademicIndex(int index, bool wrap) => (MonthOfYear)(((wrap ? index % 12 : index) + 6) % 12); // add boundary exceptions if wrap is false

    public static MonthOfYear Next(this MonthOfYear month) => FromCalendarIndex(month.ToCalendarIndex() + 1, true);
    public static MonthOfYear Previous(this MonthOfYear month) => FromCalendarIndex(month.ToCalendarIndex() - 1, true);
}