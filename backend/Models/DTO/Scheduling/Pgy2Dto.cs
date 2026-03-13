using MedicalDemo.Enums;
using MedicalDemo.Extensions;

namespace MedicalDemo.Models.DTO.Scheduling;

public class Pgy2Dto : ResidentDto
{
    public override int Pgy { get; protected set; } = 2;
    public override bool CanWork(DateOnly curDay)
    {
        if (CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(curDay, Pgy) is not
            { } shiftType)
        {
            return false;
        }

        if (IsVacation(curDay, shiftType.GetPartsOfDay()) || CommitedWorkDay(curDay))
        {
            return false;
        }

        int monthIndex = (curDay.Month + 5) % 12;
        HospitalRole? role = RolePerMonth[monthIndex];

        // Back to back check
        if (IsBackToBackShift(curDay) || IsInARowShift(curDay, shiftType))
        {
            return false;
        }

        // Rotation check
        if (!DoesRotationAllow(curDay, shiftType.GetLengthType()))
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
                "Resident already scheduled this day");
        }

        WorkDays.Add(curDay);
    }

    public override void RemoveWorkDay(DateOnly curDay)
    {
        if (!WorkDays.Contains(curDay))
        {
            throw new InvalidOperationException(
                "Resident not scheduled this day");
        }

        WorkDays.Remove(curDay);
    }
}