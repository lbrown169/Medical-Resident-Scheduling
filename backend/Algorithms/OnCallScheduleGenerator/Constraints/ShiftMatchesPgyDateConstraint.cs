using MedicalDemo.Enums;
using MedicalDemo.Extensions;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Scheduling;

namespace MedicalDemo.Algorithms.OnCallScheduleGenerator.Constraints;

public class ShiftMatchesPgyDateConstraint : ICallShiftConstraint
{
    public ConstraintResult Evaluate(ResidentDto resident, DateOnly date, CallShiftType shiftType)
    {
        CallShiftType? applicableShiftType = CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(date, resident.Pgy);

        if (shiftType == CallShiftType.Custom || shiftType == applicableShiftType)
        {
            return ConstraintResult.NoViolation();
        }

        // if shiftType is null, no valid call type found given PGYear and date
        return ConstraintResult.Violation($"Call type \"{shiftType.GetDisplayName()}\" is not valid for Resident {resident.ResidentId}, PGY: {resident.Pgy}, on date {date}.", false);
    }
}