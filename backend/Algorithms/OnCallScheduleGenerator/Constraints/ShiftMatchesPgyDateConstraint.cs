using MedicalDemo.Enums;
using MedicalDemo.Extensions;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Scheduling;

namespace MedicalDemo.Algorithms.OnCallScheduleGenerator.Constraints;

public class ShiftMatchesPgyDateConstraint : ICallShiftConstraint
{
    public ConstraintResult Evaluate(ResidentDto resident, DateOnly date)
    {
        CallShiftType? shiftType = CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(date, resident.Pgy);

        if (shiftType is not null)
        {
            return ConstraintResult.NoViolation();
        }

        // if shiftType is null, no valid call type found given PGYear and date
        return ConstraintResult.Violation($"No valid call type shift assignment available for Resident {resident.Name}, PGY: {resident.Pgy}, on date {date}.", false);
    }
}