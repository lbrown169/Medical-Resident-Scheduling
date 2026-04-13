using MedicalDemo.Enums;
using MedicalDemo.Extensions;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Scheduling;

namespace MedicalDemo.Algorithms.OnCallScheduleGenerator.Constraints;

public class ShiftMatchesRotationConstraint : ICallShiftConstraint
{
    public ConstraintResult Evaluate(ResidentDto resident, DateOnly date, CallShiftType shiftType)
    {
        CallLengthType lengthType = shiftType switch
        {
            CallShiftType.Custom => CallLengthType.Long,
            _ => shiftType.GetLengthType()
        };
        HospitalRole role = resident.GetHospitalRoleForCalendarMonth(date.Month);

        if (DoesRotationAllow(role, date, lengthType))
        {
            return ConstraintResult.NoViolation();
        }

        return ConstraintResult.Violation($"Call shift \"{shiftType.GetDisplayName()}\" disagrees with {resident.Name}'s {role.Name} rotation.", true);
    }

    private bool DoesRotationAllow(HospitalRole role, DateOnly date, CallLengthType lengthType)
    {
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