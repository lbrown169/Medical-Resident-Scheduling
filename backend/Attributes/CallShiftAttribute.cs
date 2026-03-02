using MedicalDemo.Algorithms.OnCallScheduleGenerator;
using MedicalDemo.Enums;

namespace MedicalDemo.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class CallShiftAttribute : Attribute
{
    public required int Hours { get; init; }
    public required CallLengthType CallLengthType { get; init; }
    public int RequiredPgy { get; init; } = 0;
    public DayOfWeek[]? ApplicableDays { get; init; } = null;
    public CallShiftRule DateRule { get; init; } = CallShiftRule.None;
    public int Priority { get; init; } = 0;
}