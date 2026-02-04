using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Scheduling;

namespace MedicalDemo.Algorithm;

public class PGY2
{
    private readonly HashSet<DateOnly>
        allWorkDates; // every single day they are supposed to work

    private readonly HashSet<DateOnly>
        commitedWorkDays; // days that have been committed to work by some prior schedule (don't change past)

    private readonly HashSet<DateOnly> vacationRequests;

    private readonly int hoursWorked6months;
    private readonly int hoursWorkedTotal;

    public string id;

    // name, vacation, hours DO THEY HAVE HOSPITAL ROLES?
    public string name; // public to be accessible outside the class
    public HospitalRole[] rolePerMonth; // the PGY2's role per month

    public PGY2(string name)
    {
        this.name = name;
        rolePerMonth
            = new HospitalRole[12]; //dynamic memory allocation in c#
        vacationRequests = new HashSet<DateOnly>();
        allWorkDates = new HashSet<DateOnly>(); // initialize the hashset
        commitedWorkDays
            = new HashSet<DateOnly>(); // initialize the hashset
        hoursWorked6months = 0;
        hoursWorkedTotal = 0;
        inTraining = true;
        commitedWorkDays = new HashSet<DateOnly>();
    }


    public bool inTraining { get; set; }

    // check if the pgy1 is requesting vacation on curDay
    public bool isVacation(DateOnly curDay)
    {
        return vacationRequests.Contains(curDay);
    }

    // add a vacation request
    public void requestVacation(DateOnly curDay)
    {
        if (isVacation(curDay))
        {
            Console.WriteLine(
                "warning: you already requested vacation for this day.");
            return;
        }

        vacationRequests.Add(curDay);
    }

    // return if the pgy1 is working on curDay
    public bool isWorking(DateOnly curDay)
    {
        return allWorkDates.Contains(curDay);
    }

    public void removeWorkDay(DateOnly curDay)
    {
        allWorkDates.Remove(curDay);
    }

    public bool addWorkDay(DateOnly curDay)
    {
        if (allWorkDates.Contains(curDay))
        {
            Console.WriteLine("error: the resident already works this day?");
            return false;
        }

        allWorkDates.Add(curDay);
        return true;
    }

    // return true if the pgy1 can work on curDay
    public bool canWork(DateOnly curDay)
    {
        // check if the PGY1 is in training and not on vacation
        if (isVacation(curDay))
        {
            return false;
        }

        // role check : check that the role allows for working this type of shift
        if (rolePerMonth[(curDay.Month + 5) % 12] == null)
        {
            Console.WriteLine(
                "warning: you have no role assigned for this month.");
            return false;
        }

        // check if curday is long call (saturday/sunday)
        if (curDay.DayOfWeek == DayOfWeek.Saturday ||
            curDay.DayOfWeek == DayOfWeek.Sunday)
        {
            // check if the role allows for long call
            if (!rolePerMonth[(curDay.Month + 5) % 12].DoesLong &&
                (!inTraining || !rolePerMonth[(curDay.Month + 5) % 12]
                    .DoesTrainingLong))
            {
                return false;
            }

            // check that we don't work consecutive weekend days
            DateOnly previousDay = curDay.AddDays(-1);
            DateOnly nextDay = curDay.AddDays(1);
            if (isWorking(previousDay) || isWorking(nextDay))
            {
                return false;
            }
        }

        // check for short call (any weekday)
        if (curDay.DayOfWeek == DayOfWeek.Monday ||
            curDay.DayOfWeek == DayOfWeek.Tuesday ||
            curDay.DayOfWeek == DayOfWeek.Wednesday ||
            curDay.DayOfWeek == DayOfWeek.Thursday ||
            curDay.DayOfWeek == DayOfWeek.Friday)
        {
            // check if the role allows for short call
            if (!rolePerMonth[(curDay.Month + 5) % 12].DoesShort &&
                (!inTraining || !rolePerMonth[(curDay.Month + 5) % 12]
                    .DoesTrainingShort))
            {
                return false;
            }

            // check that we don't work consecutive to a weekend (long call)
            DateOnly previousDay = curDay.AddDays(-1);
            DateOnly nextDay = curDay.AddDays(1);
            if (isWorking(nextDay) &&
                nextDay.DayOfWeek == DayOfWeek.Saturday)
            {
                return false;
            }

            if (isWorking(previousDay) &&
                previousDay.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }
        }

        // we can work that day
        return true;
    }


    public DateOnly lastWorkDay()
    {
        // check if the hashset of allworkdays is empty
        if (allWorkDates.Count == 0)
        {
            return new DateOnly(1, 1, 1); // or throw an exception
        }

        DateOnly ret = new(1, 1, 1); // initialize to a far past date
        foreach (DateOnly cur in allWorkDates)
        {
            if (ret < cur)
            {
                ret = cur;
            }
        }

        return ret;
    }

    public DateOnly firstWorkDay()
    {
        // check if the hashset of allworkdays is empty
        if (allWorkDates.Count == 0)
        {
            return new DateOnly(2, 2, 2); // or throw an exception
        }

        DateOnly ret = new(9999, 12, 31); // initialize to a far future date
        foreach (DateOnly cur in allWorkDates)
        {
            if (ret > cur)
            {
                ret = cur;
            }
        }

        return ret;
    }

    public HashSet<DateOnly> workDaySet()
    {
        // return a copy of the work days
        return new HashSet<DateOnly>(allWorkDates);
    }

    public bool commitedWorkDay(DateOnly curDay)
    {
        // TODO: when integrating consider database look up
        // check if this day has been committed to work by some prior schedule
        if (commitedWorkDays.Contains(curDay))
        {
            return true;
        }

        return false;
    }

    public void saveWorkDays()
    {
        foreach (DateOnly curDay in allWorkDates)
        // TODO: save the work day to a file or database
        // this is a placeholder for the actual implementation
        {
            commitedWorkDays.Add(curDay);
        }

        // Console.WriteLine("SAVE WORK DAYS NOT FULLY IMPLEMENTED");
    }
}