using MedicalDemo.Enums;
using MedicalDemo.Extensions;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Scheduling;

namespace MedicalDemo.Algorithms.OnCallScheduleGenerator.Constraints;

public class NotOnVacationConstraint : ICallShiftConstraint
{
    public ConstraintResult Evaluate(ResidentDto resident, DateOnly date)
    {
        CallShiftType? shiftType = CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(date, resident.Pgy);

        if (shiftType is null)
        {
            throw new InvalidOperationException(
                "Unable to determine Resident shiftType - invalid");
        }

        if (shiftType is not null && IsVacation(resident, date, shiftType.Value.GetPartsOfDay()))
        {
            return ConstraintResult.Violation($"Resident {resident} is on vacation on {date}.",
                true);
        }

        return ConstraintResult.NoViolation();
    }

    private bool IsVacation(ResidentDto resident, DateOnly date, PartOfDay partOfDay)
    {
        if (partOfDay.HasFlag(PartOfDay.Morning) && IsMorningVacation(resident, date))
        {
            return true;
        }

        if (partOfDay.HasFlag(PartOfDay.Afternoon) && IsAfternoonVacation(resident, date))
        {
            return true;
        }

        return false;
    }

    private bool IsMorningVacation(ResidentDto resident, DateOnly curDay)
    {
        return resident.MorningVacationRequests.Contains(curDay);
    }

    private bool IsAfternoonVacation(ResidentDto resident, DateOnly curDay)
    {
        return resident.AfternoonVacationRequests.Contains(curDay);
    }
}