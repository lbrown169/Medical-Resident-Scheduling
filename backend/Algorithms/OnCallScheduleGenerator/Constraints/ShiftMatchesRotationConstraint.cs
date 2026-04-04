using MedicalDemo.Enums;
using MedicalDemo.Extensions;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Scheduling;

namespace MedicalDemo.Algorithms.OnCallScheduleGenerator.Constraints;

public class ShiftMatchesRotationConstraint : ICallShiftConstraint
{
    public ConstraintResult Evaluate(ResidentDto resident, DateOnly date)
    {
        CallShiftType? shiftType = CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(date, resident.Pgy);

        if (shiftType is not null && DoesRotationAllow(resident, date, shiftType.Value.GetLengthType()))
        {
            return ConstraintResult.NoViolation();
        }

        return ConstraintResult.Violation($"Resident {resident} shift disagrees with their rotation.", true);
    }

    private bool DoesRotationAllow(ResidentDto resident, DateOnly date, CallLengthType lengthType)
    {
        HospitalRole role = resident.GetHospitalRoleForCalendarMonth(date.Month);
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
}