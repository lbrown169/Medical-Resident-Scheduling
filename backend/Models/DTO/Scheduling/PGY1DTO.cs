using MedicalDemo.Enums;

namespace MedicalDemo.Models.DTO.Scheduling;

public class PGY1DTO : ResidentDTO
{
    public DateOnly LastTrainingDate { get; set; }

    public override bool CanWork(DateOnly curDay, CallLengthType lengthType)
    {
        if (IsVacation(curDay) || CommitedWorkDay(curDay))
        {
            return false;
        }

        int monthIndex = (curDay.Month + 5) % 12;
        HospitalRole? role = RolePerMonth[monthIndex];

        if (lengthType == CallLengthType.Long)
        {
            if (role is { DoesLong: false } && (!InTraining || !role.DoesTrainingLong))
            {
                return false;
            }

            DateOnly prevDay = curDay.AddDays(-1);
            DateOnly nextDay = curDay.AddDays(1);
            if (WorkDays.Contains(prevDay) ||
                WorkDays.Contains(nextDay))
            {
                return false;
            }
        }
        else // Weekday
        {
            if (role is { DoesShort: false } && (!InTraining || !role.DoesTrainingShort))
            {
                return false;
            }

            DateOnly nextDay = curDay.AddDays(1);
            DateOnly prevDay = curDay.AddDays(-1);

            if (WorkDays.Contains(nextDay) &&
                nextDay.DayOfWeek == DayOfWeek.Saturday)
            {
                return false;
            }

            if (WorkDays.Contains(prevDay) &&
                prevDay.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }
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