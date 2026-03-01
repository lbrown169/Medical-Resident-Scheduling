using System.Reflection;
using MedicalDemo.Attributes;
using MedicalDemo.Enums;
using MedicalDemo.Models.DTO.Scheduling;

namespace MedicalDemo.Extensions;

public static class CallShiftTypeExtensions
{
    private static readonly Dictionary<CallShiftType, CallShiftAttribute> _cache =
        Enum.GetValues<CallShiftType>()
            .ToDictionary(
                shift => shift,
                shift => typeof(CallShiftType)
                             .GetField(shift.ToString())!
                             .GetCustomAttribute<CallShiftAttribute>())
            .Where(kvp => kvp.Value is not null)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value!);

    private static CallShiftAttribute Attr(this CallShiftType shift) => _cache[shift];

    public static List<CallShiftType> GetAllAlgorithmCallShiftTypes()
    {
        return _cache.Keys.ToList();
    }

    public static List<CallShiftType> GetAllAlgorithmCallShiftTypesForDate(DateOnly date)
    {
        List<CallShiftType> matches = GetAllAlgorithmCallShiftTypes().Where(s =>
        {
            CallShiftAttribute attr = s.Attr();
            bool dayMatches = attr.ApplicableDays is null ||
                              attr.ApplicableDays.Contains(date.DayOfWeek);
            bool ruleMatches = attr.DateRule.Applies(date);
            return dayMatches && ruleMatches;
        }).ToList();

        if (matches.Count == 0)
        {
            return matches;
        }

        int maxPriority = matches.Max(s => s.Attr().Priority);
        return matches.Where(s => s.Attr().Priority == maxPriority).ToList();
    }

    public static CallShiftType? GetAlgorithmCallShiftTypeForDate(DateOnly date, int year)
    {
        return GetAllAlgorithmCallShiftTypesForDate(date).Cast<CallShiftType?>().FirstOrDefault(s =>
        {
            int requiredPgy = ((CallShiftType)s!).Attr().RequiredPgy;
            return requiredPgy == 0 || requiredPgy == year;
        });
    }

    public static List<CallShiftType> GetAllAlgorithmCallShiftsForYear(int year)
    {
        return GetAllAlgorithmCallShiftTypes().Where(s =>
        {
            int requiredPgy = s.Attr().RequiredPgy;
            return requiredPgy == 0 || requiredPgy == year;
        }).ToList();
    }

    public static int? GetRequiredYear(this CallShiftType shift) => shift.Attr().RequiredPgy == 0 ? null : shift.Attr().RequiredPgy;

    public static int GetHours(this CallShiftType shift) => shift.Attr().Hours;

    public static CallLengthType GetLengthType(this CallShiftType shift) => shift.Attr().CallLengthType;
}