using MedicalDemo.Enums;
using MedicalDemo.Extensions;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Scheduling;

namespace MedicalDemo.Algorithms.OnCallScheduleGenerator.Constraints;

public class NotOnVacationConstraint : ICallShiftConstraint
{
    public ConstraintResult Evaluate(ResidentDto resident, DateOnly date, CallShiftType shiftType)
    {
        PartOfDay partOfDay = shiftType is CallShiftType.Custom ? PartOfDay.AllDay : shiftType.GetPartsOfDay();
        if (IsVacation(resident, date, partOfDay))
        {
            return ConstraintResult.Violation($"Resident {resident.ResidentId} is on vacation on {date}.", true);
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