public class PGY3
{
    private readonly HashSet<DateOnly>
        allWorkDates; // every single day they are supposed to work

    private readonly HashSet<DateOnly> committedWorkDates; //dates they are already working

    private readonly HashSet<DateOnly> vacationRequests;

    private readonly int hoursWorked6months;
    private readonly int hoursWorkedTotal;

    public string id;

    // name, vacation, hours DO THEY HAVE HOSPITAL ROLES?
    public string name; // public to be accessible outside the class

    public PGY3(string name)
    {
        this.name = name;
        vacationRequests = new HashSet<DateOnly>();
        allWorkDates = new HashSet<DateOnly>(); // initialize the hashset
        committedWorkDates = [];
        hoursWorked6months = 0;
        hoursWorkedTotal = 0;
    }


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

    public void removeCommittedWorkDay(DateOnly curDay)
    {
        committedWorkDates.Remove(curDay);
    }

    public bool addCommittedWorkDay(DateOnly curDay)
    {
        if (committedWorkDates.Contains(curDay))
        {
            Console.WriteLine("error: the resident already works this day?");
            return false;
        }

        committedWorkDates.Add(curDay);
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

        // check if curday is long call (saturday/sunday)
        if (curDay.DayOfWeek == DayOfWeek.Saturday ||
            curDay.DayOfWeek == DayOfWeek.Sunday)
        {
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

    public void saveWorkDays()
    {
        // Console.WriteLine("SAVE WORK DAYS NOT IMPLEMENTED");
    }
}