using MedicalDemo.Enums;

namespace MedicalDemo.Models.DTO.Scheduling;

public class Pgy2Dto : ResidentDto
{
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
            if (curDay.Month is 7 or 8 && role is { DoesTrainingLong: false } ||
                curDay.Month is not 7 and not 8 && role is { DoesLong: false })
            {
                return false;
            }

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
            if (curDay.Month is 7 or 8 && role is { DoesTrainingShort: false } ||
                curDay.Month is not 7 and not 8 && role is { DoesShort: false })
            {
                return false;
            }

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