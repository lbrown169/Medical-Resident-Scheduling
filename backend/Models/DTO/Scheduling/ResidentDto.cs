using MedicalDemo.Enums;
using MedicalDemo.Extensions;

namespace MedicalDemo.Models.DTO.Scheduling;

public abstract class ResidentDto
{
    public string ResidentId { get; set; }
    public string Name { get; set; }
    public abstract int Pgy { get; protected set; }
    public bool InTraining { get; set; }
    public HashSet<DateOnly> MorningVacationRequests { get; set; } = new();
    public HashSet<DateOnly> AfternoonVacationRequests { get; set; } = new();
    public HashSet<DateOnly> WorkDays { get; set; } = new();
    public HashSet<DateOnly> CommitedWorkDays { get; set; } = new();
    public HashSet<DateOnly> PendingSaveWorkDays { get; set; } = new();
    public HashSet<DateOnly> AllPendingWorkDays => [.. PendingSaveWorkDays, .. WorkDays];

    public HospitalRole?[] RolePerMonth { get; set; } = new HospitalRole?[12];

    public abstract bool CanWork(DateOnly date);
    public abstract void AddWorkDay(DateOnly date);
    public abstract void RemoveWorkDay(DateOnly date);

    public bool CommitedWorkDay(DateOnly curDay)
    {
        return CommitedWorkDays.Contains(curDay);
    }

    public void SaveWorkDays()
    {
        for (int i = WorkDays.Count - 1; i >= 0; i--)
        {
            DateOnly day = WorkDays.ElementAt(i);
            CommitedWorkDays.Add(day);
            PendingSaveWorkDays.Add(day);
            WorkDays.Remove(day);
        }
    }

    public DateOnly LastWorkDay()
    {
        return WorkDays.Count > 0
            ? WorkDays.Max()
            : new DateOnly(1, 1, 1);
    }

    public DateOnly FirstWorkDay()
    {
        return WorkDays.Count > 0
            ? WorkDays.Min()
            : new DateOnly(9999, 12, 31);
    }

    public bool IsMorningVacation(DateOnly curDay)
    {
        return MorningVacationRequests.Contains(curDay);
    }

    public bool IsAfternoonVacation(DateOnly curDay)
    {
        return AfternoonVacationRequests.Contains(curDay);
    }

    public bool IsVacation(DateOnly date, PartOfDay partOfDay)
    {
        if (partOfDay.HasFlag(PartOfDay.Morning) && IsMorningVacation(date))
        {
            return true;
        }

        if (partOfDay.HasFlag(PartOfDay.Afternoon) && IsAfternoonVacation(date))
        {
            return true;
        }

        return false;
    }

    public bool IsWorking(DateOnly curDay)
    {
        return WorkDays.Contains(curDay);
    }

    public bool CanAddWorkDay(DateOnly curDay)
    {
        return CanWork(curDay) && !IsWorking(curDay);
    }

    protected bool IsBackToBackShift(DateOnly date)
    {
        DateOnly prevDay = date.AddDays(-1);
        DateOnly nextDay = date.AddDays(1);

        return IsWorking(prevDay) || CommitedWorkDay(nextDay) || IsWorking(nextDay) || CommitedWorkDay(prevDay);
    }

    protected bool IsInARowShift(DateOnly date, CallShiftType type)
    {
        // Walk backwards at most a week
        DateOnly until = date.AddDays(-7);
        for (DateOnly processing = date.AddDays(-1); processing >= until; processing = processing.AddDays(-1))
        {
            CallShiftType? processingType = CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(processing, Pgy);
            if (processingType == type)
            {
                if (IsWorking(processing) || CommitedWorkDay(processing))
                {
                    return true;
                }

                break;
            }
        }

        until = date.AddDays(7);
        for (DateOnly processing = date.AddDays(1); processing <= until; processing = processing.AddDays(1))
        {
            CallShiftType? processingType = CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(processing, Pgy);
            if (processingType == type)
            {
                if (IsWorking(processing) || CommitedWorkDay(processing))
                {
                    return true;
                }

                break;
            }
        }

        return false;
    }

    protected bool DoesRotationAllow(DateOnly date, CallLengthType lengthType)
    {
        HospitalRole role = GetHospitalRoleForCalendarMonth(date.Month);
        if (lengthType == CallLengthType.Long)
        {
            if (date.Month is 7 or 8 && role is { DoesTrainingLong: false } ||
                date.Month is not 7 and not 8 && role is { DoesLong: false })
            {
                return false;
            }
        }
        else // Weekday
        {
            if (date.Month is 7 or 8 && role is { DoesTrainingShort: false } ||
                date.Month is not 7 and not 8 && role is { DoesShort: false })
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    ///     Get hospital role for a resident for a month.
    /// </summary>
    /// <param name="index">Academic year indexed, July is 0.</param>
    /// <returns></returns>
    public HospitalRole GetHospitalRoleForAcademicMonth(int index)
    {
        if (RolePerMonth is null || RolePerMonth.Length < index + 1)
        {
            return HospitalRole.Unassigned;
        }
        return RolePerMonth[index] ?? HospitalRole.Unassigned;
    }

    public HospitalRole GetHospitalRoleForCalendarMonth(int index)
    {
        return GetHospitalRoleForAcademicMonth((index + 5) % 12);
    }
}