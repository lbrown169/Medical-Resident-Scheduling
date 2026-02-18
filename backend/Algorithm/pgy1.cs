using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Scheduling;

namespace MedicalDemo.Algorithm;

public class PGY1
{
    private readonly HashSet<DateOnly>
        allWorkDates; // every single day they are supposed to work

    private readonly HashSet<DateOnly>
        commitedWorkDays; // days that have been committed to work by some prior schedule (don't change past)

    private readonly HashSet<DateOnly> vacationRequests;

    private readonly int hoursWorked6months; // database
    private readonly int hoursWorkedTotal;
    public string id;
    public string name; // public to be accessible outside the class
    public HospitalRole[] rolePerMonth; // the PGY1's role per month

    public PGY1(string name)
    {
        this.name = name;
        rolePerMonth
            = new HospitalRole[12]; //dynamic memory allocation in c#
        vacationRequests = [];
        allWorkDates = []; // initialize the hashset
        hoursWorked6months = 0;
        hoursWorkedTotal = 0;
        inTraining = true; // move to 0 after all training complete
        lastTrainingDate = new DateOnly(1, 1, 1); // far past date
        commitedWorkDays = [];
    }

    public DateOnly
        lastTrainingDate
    {
        get;
        set;
    } // this is OOP but i didnt wind up using it. but it cant hurt

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
    public bool
        canWork(
            DateOnly curDay) // function fixes the sponsors desires to not have
    {
        // back to back days worked. also handles vacations
        // check if the PGY1 is in training and not on vacation
        if (isVacation(curDay))
        {
            return false;
        }

        // role check : check that the role allows for working this type of shift
        if (rolePerMonth[(curDay.Month + 5) % 12] == null)
        {
            Console.WriteLine(
                $"warning: {name} have no role assigned for this month {curDay} {curDay.Month}.");
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

        if (curDay <= lastTrainingDate)
        {
            return false;
        }

        // we can work that day
        return true;
    }

    public DateOnly
        lastWorkDay() // used to make sure theyre not working back to back long calls
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

    public DateOnly
        firstWorkDay() // this function is used to find the earliest day worked
    {
        // so that i can iterate through all days worked
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

    public HashSet<DateOnly> workDaySet() // all worked days
    {
        // return a copy of the work days
        return [.. allWorkDates];
    }

    public bool
        commitedWorkDay(
            DateOnly curDay) // days that were worked in a previous 6 months
    {
        // that shouldnt be changed when generating next 6
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