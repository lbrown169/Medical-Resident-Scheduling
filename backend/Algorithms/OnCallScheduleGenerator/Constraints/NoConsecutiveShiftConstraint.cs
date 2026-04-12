using MedicalDemo.Enums;
using MedicalDemo.Extensions;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Scheduling;

namespace MedicalDemo.Algorithms.OnCallScheduleGenerator.Constraints;

public class NoConsecutiveShiftConstraint : ICallShiftConstraint
{
    public ConstraintResult Evaluate(ResidentDto resident, DateOnly date, CallShiftType shiftType)
    {
        if (IsBackToBackShift(resident, date))
        {
            return ConstraintResult.Violation($"Resident {resident.Name} ({resident.ResidentId}) will be working a back-to-back shift", true);
        }

        if (IsInARowShift(resident, date, shiftType))
        {
            return ConstraintResult.Violation($"Resident {resident.Name} ({resident.ResidentId}) will be working an in-a-row shift", true);
        }

        return ConstraintResult.NoViolation();
    }

    private bool IsBackToBackShift(ResidentDto resident, DateOnly date)
    {
        DateOnly prevDay = date.AddDays(-1);
        DateOnly nextDay = date.AddDays(1);

        return resident.IsWorking(prevDay) || resident.CommitedWorkDay(nextDay) || resident.IsWorking(nextDay) || resident.CommitedWorkDay(prevDay);
    }

    private bool IsInARowShift(ResidentDto resident, DateOnly date, CallShiftType type)
    {
        // Walk backwards at most a week
        DateOnly until = date.AddDays(-7);
        for (DateOnly processing = date.AddDays(-1); processing >= until; processing = processing.AddDays(-1))
        {
            CallShiftType? processingType = CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(processing, resident.Pgy);
            if (processingType == type)
            {
                if (resident.IsWorking(processing) || resident.CommitedWorkDay(processing))
                {
                    return true;
                }

                break;
            }
        }

        until = date.AddDays(7);
        for (DateOnly processing = date.AddDays(1); processing <= until; processing = processing.AddDays(1))
        {
            CallShiftType? processingType = CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(processing, resident.Pgy);
            if (processingType == type)
            {
                if (resident.IsWorking(processing) || resident.CommitedWorkDay(processing))
                {
                    return true;
                }

                break;
            }
        }

        return false;
    }
}