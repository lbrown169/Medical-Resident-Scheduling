using MedicalDemo.Enums;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Scheduling;

namespace MedicalDemo.Algorithms.OnCallScheduleGenerator.Constraints;

public class OneShiftADayConstraint : ICallShiftConstraint
{
    public ConstraintResult Evaluate(ResidentDto resident, DateOnly date, CallShiftType shiftType)
    {
        if (IsWorking(resident, date))
        {
            return ConstraintResult.Violation($"Resident {resident.Name} has a scheduled shift on {date}.", false);
        }

        return ConstraintResult.NoViolation();
    }

    private bool IsWorking(ResidentDto resident, DateOnly curDay)
    {
        return (resident.IsWorking(curDay) || resident.CommitedWorkDay(curDay)) && !resident.IsPendingRemoval(curDay);
    }
}