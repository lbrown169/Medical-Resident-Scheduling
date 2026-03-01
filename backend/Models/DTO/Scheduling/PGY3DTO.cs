using MedicalDemo.Enums;

namespace MedicalDemo.Models.DTO.Scheduling;

public class PGY3DTO : ResidentDTO
{
    public override bool CanWork(DateOnly curDay, CallLengthType lengthType)
    {
        if (IsVacation(curDay) || CommitedWorkDay(curDay))
        {
            return false;
        }

        if (lengthType == CallLengthType.Long)
        {
            DateOnly prevDay = curDay.AddDays(-1);
            DateOnly nextDay = curDay.AddDays(1);
            if (IsWorking(prevDay)
                || CommitedWorkDay(nextDay)
                || IsWorking(nextDay)
                || CommitedWorkDay(prevDay))
            {
                return false;
            }
        }
        else // Weekday
        {
            DateOnly nextDay = curDay.AddDays(1);
            DateOnly prevDay = curDay.AddDays(-1);

            if ((IsWorking(nextDay) || CommitedWorkDay(nextDay)) &&
                nextDay.DayOfWeek == DayOfWeek.Saturday)
            {
                return false;
            }

            if ((IsWorking(prevDay) || CommitedWorkDay(prevDay)) &&
                prevDay.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }
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