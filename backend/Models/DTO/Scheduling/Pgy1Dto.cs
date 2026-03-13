using MedicalDemo.Enums;
using MedicalDemo.Extensions;

namespace MedicalDemo.Models.DTO.Scheduling;

public class Pgy1Dto : ResidentDto
{
    public override int Pgy { get; protected set; } = 1;
    public DateOnly LastTrainingDate { get; set; }

    public override bool CanWork(DateOnly curDay)
    {
        if (IsVacation(curDay) || CommitedWorkDay(curDay))
        {
            return false;
        }

        // Back to back check
        if (CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(curDay, Pgy) is not
            { } shiftType)
        {
            return false;
        }

        if (IsBackToBackShift(curDay) || IsInARowShift(curDay, shiftType))
        {
            return false;
        }

        // Rotation check
        if (!DoesRotationAllow(curDay, shiftType.GetLengthType()))
        {
            return false;
        }

        if (curDay <= LastTrainingDate)
        {
            return false;
        }

        return true;
    }

    public override void AddWorkDay(DateOnly curDay)
    {
        if (WorkDays.Contains(curDay))
        {
            throw new InvalidOperationException(
                $"Resident already scheduled for {curDay}");
        }

        WorkDays.Add(curDay);
    }

    public override void RemoveWorkDay(DateOnly curDay)
    {
        if (!WorkDays.Contains(curDay))
        {
            throw new InvalidOperationException(
                $"Resident not scheduled for {curDay}");
        }

        WorkDays.Remove(curDay);
    }
}