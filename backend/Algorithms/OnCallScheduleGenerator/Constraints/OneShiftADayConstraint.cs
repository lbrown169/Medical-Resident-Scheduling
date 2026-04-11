using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Scheduling;

namespace MedicalDemo.Algorithms.OnCallScheduleGenerator.Constraints;

public class OneShiftADayConstraint : ICallShiftConstraint
{
    public ConstraintResult Evaluate(ResidentDto resident, DateOnly date)
    {
        if (IsWorking(resident, date))
        {
            return ConstraintResult.Violation($"Resident {resident.ResidentId} has a scheduled shift on {date}.", false);
        }

        return ConstraintResult.NoViolation();
    }

    private bool IsWorking(ResidentDto resident, DateOnly curDay)
    {
        return resident.WorkDays.Contains(curDay);
    }

    public bool IsApplicable(bool isDateUpdate, bool isResidentUpdate) => isDateUpdate || isResidentUpdate;
}