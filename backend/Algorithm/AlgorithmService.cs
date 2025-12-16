using System.Collections;
using MedicalDemo.Models.DTO.Scheduling;

//for array min/max

namespace MedicalDemo.Algorithm;

public class AlgorithmService
{
    private readonly ILogger<AlgorithmService> _logger;

    public AlgorithmService(ILogger<AlgorithmService> logger)
    {
        _logger = logger;
    }

    // === NEW OVERLOAD METHODS FOR BACKEND INTEGRATION ===
    public bool Training(int year, List<PGY1> pgy1s, List<PGY2> pgy2s,
        List<PGY3> pgy3s)
    {
        ArrayList pgys1 = new(pgy1s);
        ArrayList pgys2 = new(pgy2s);
        ArrayList pgys3 = new(pgy3s);
        return Training(year, pgys1, pgys2, pgys3);
    }

    public bool Part1(int year, List<PGY1> pgy1s, List<PGY2> pgy2s)
    {
        ArrayList pgys1 = new(pgy1s);
        ArrayList pgys2 = new(pgy2s);
        return Part1(year, pgys1, pgys2);
    }

    public bool Part2(int year, List<PGY1> pgy1s, List<PGY2> pgy2s)
    {
        ArrayList pgys1 = new(pgy1s);
        ArrayList pgys2 = new(pgy2s);
        return Part2(year, pgys1, pgys2);
    }

    public bool Training(int year, ArrayList pgy1s, ArrayList pgy2s,
        ArrayList pgy3s)
    {
        int pgy1 = 8;
        int pgy2 = 8;
        int pgy3 = 8;
        ArrayList AllPgy1s = pgy1s;
        ArrayList AllPgy2s = pgy2s;
        ArrayList AllPgy3s = pgy3s;
        TrainingCalendar tCalendar = new(year);

        int Sat24hCallAmt
            = tCalendar.dayOfWeekAmt[6]; // how many saturday calls for training
        int Sun12hCallAmt = tCalendar.dayOfWeekAmt[0]; // how many sunday calls
        int shortCallAmt =
            tCalendar.dayOfWeekAmt[2] + tCalendar.dayOfWeekAmt[3] +
            tCalendar.dayOfWeekAmt[4]; // t + w + th

        // make sure a pgy1 is with a pgy3 3 times for a short call
        int nodeAmt
            = shortCallAmt * 2 /* split node */ + pgy3 + pgy1 +
              2 /*source & sink*/; // !!!!node id order!!!!
        int sourceIndex = nodeAmt - 2; // -2 for 0 indexing
        int sinkIndex = nodeAmt - 1;

        Graph shortCallGraph = new(nodeAmt);
        for (int i = 0; i < pgy1; i++)
        {
            shortCallGraph.addEdge(sourceIndex, shortCallAmt * 2 + pgy3 + i,
                3);
        }
        //source node, pgy1 index, days pgy1s need to work

        for (int i = 0; i < pgy1; i++)
        {
            for (int j = 0;
                 j < shortCallAmt;
                 j++) // TO DO: THIS IS WHERE VACATION TIME WILL GO!!!!
            {
                // what month are we in
                int month = tCalendar.whatShortDayIsIt(j).Month;

                // DEBUG Console.WriteLine(month);
                //are we in july?
                if (month == 7)
                {
                    if (((PGY1)AllPgy1s[i]).rolePerMonth[0].DoesShort == false &&
                        ((PGY1)AllPgy1s[i]).rolePerMonth[0].DoesTrainingShort ==
                        false) // if their role doesnt do short calls this month
                    {
                        continue;
                    }
                }
                // then skip over them and dont add the edge because they cant be used

                // are we in august?
                if (month == 8)
                {
                    if (((PGY1)AllPgy1s[i]).rolePerMonth[1].DoesShort == false &&
                        ((PGY1)AllPgy1s[i]).rolePerMonth[1].DoesTrainingShort == false)
                    {
                        continue;
                    }
                }
                // same as above

                shortCallGraph.addEdge(shortCallAmt * 2 + pgy3 + i, 2 * j,
                    1); //each pgy1 has an edge between them and a training day
            }
        }

        for (int i = 0; i < shortCallAmt; i++)
        {
            shortCallGraph.addEdge(i * 2, i * 2 + 1,
                1);
        }
        // connecting split nodes.

        for (int i = 0; i < shortCallAmt; i++)
        {
            for (int j = 0;
                 j < pgy3;
                 j++) // TO DO: THIS IS WHERE VACATION TIME WILL GO!!!!
            {
                shortCallGraph.addEdge(i * 2 + 1, shortCallAmt * 2 + j, 1);
            }
        }
        //each pgy3 can only train once per day

        int estPgy3Training
            = (3 * pgy1 + pgy3 - 1) /
              pgy3; // ceiling division to get capacity amount for sink
        for (int i = 0; i < pgy3; i++)
        {
            shortCallGraph.addEdge(shortCallAmt * 2 + i, sinkIndex,
                estPgy3Training);
        }

        if (shortCallGraph.getFlow(sourceIndex, sinkIndex) != 3 * pgy1)
        {
            Console.WriteLine(
                "[ERROR] Not able to make valid assignment based on parameters");
            return false;
        }

        Console.WriteLine(
            "Succesfully created pgy3 weekday training assignment");
        //Console.WriteLine("PGY1 Assignments:");
        for (int i = 0; i < pgy1; i++)
        {
            //Console.WriteLine($" PGY1 #{((PGY1)(AllPgy1s[i])).name}:");
            ArrayList? curList
                = (ArrayList)shortCallGraph.adjList[
                    shortCallAmt * 2 + pgy3 + i];
            for (int j = 0; j < curList.Count; j++)
            {
                Edge? currEdge = (Edge)curList[j];
                // check if flow is leaving the current edge
                if (currEdge.flow() > 0)
                //Console.WriteLine($"  Day {tCalendar.whatShortDayIsIt(currEdge.destination / 2)}");
                {
                    ((PGY1)AllPgy1s[i]).addWorkDay(
                        tCalendar.whatShortDayIsIt(currEdge.destination / 2));
                }
            }
        }

        //Console.WriteLine("PGY3 Assignments:");
        for (int i = 0; i < pgy3; i++)
        {
            //Console.WriteLine($" PGY3 #{((PGY3)(AllPgy3s[i])).name}");
            ArrayList? curList
                = (ArrayList)shortCallGraph.adjList[shortCallAmt * 2 + i];
            for (int j = 0; j < curList.Count; j++)
            {
                Edge? currEdge = (Edge)curList[j];
                // check if the flow is negative
                if (currEdge.flow() < 0)
                //Console.WriteLine($"  Day {tCalendar.whatShortDayIsIt(currEdge.destination / 2)}");
                {
                    ((PGY3)AllPgy3s[i]).addWorkDay(
                        tCalendar.whatShortDayIsIt(currEdge.destination / 2));
                }
            }
        }

        // make sure a pgy1 is with a pgy2 on a 24h saturday
        nodeAmt = Sat24hCallAmt * 2 /* split node */ + pgy2 + pgy1 +
                  2 /*source & sink*/; // !!!!node id order!!!!
        sourceIndex = nodeAmt - 2; // -2 for 0 indexing
        sinkIndex = nodeAmt - 1;
        Graph saturdayCallGraph = new(nodeAmt);
        for (int i = 0; i < pgy1; i++)
        {
            saturdayCallGraph.addEdge(sourceIndex, Sat24hCallAmt * 2 + pgy2 + i,
                1);
        }
        //source node, pgy1 index, days pgy1s need to work

        for (int i = 0; i < pgy1; i++)
        {
            for (int j = 0;
                 j < Sat24hCallAmt;
                 j++) // TO DO: THIS IS WHERE VACATION TIME WILL GO!!!!
            {
                // what month are we in
                int month = tCalendar.whatSaturdayIsIt(j).Month;

                // DEBUG Console.WriteLine(month);
                //are we in july?
                if (month == 7)
                {
                    if (((PGY1)AllPgy1s[i]).rolePerMonth[0].DoesLong == false &&
                        ((PGY1)AllPgy1s[i]).rolePerMonth[0].DoesTrainingLong ==
                        false) // if their role doesnt do long calls this month
                    {
                        continue;
                    }
                }
                // then skip over them and dont add the edge because they cant be used

                // are we in august?
                if (month == 8)
                {
                    if (((PGY1)AllPgy1s[i]).rolePerMonth[1].DoesLong == false &&
                        ((PGY1)AllPgy1s[i]).rolePerMonth[1].DoesTrainingLong == false)
                    {
                        continue;
                    }
                }
                // same as above

                saturdayCallGraph.addEdge(Sat24hCallAmt * 2 + pgy2 + i, 2 * j,
                    1); //each pgy1 has an edge between them and a training day
            }
        }

        for (int i = 0; i < Sat24hCallAmt; i++)
        {
            saturdayCallGraph.addEdge(i * 2, i * 2 + 1, 1);
        }
        // connecting split nodes.

        for (int i = 0; i < pgy2; i++)
        {
            for (int j = 0;
                 j < Sat24hCallAmt;
                 j++) // TO DO: THIS IS WHERE VACATION TIME WILL GO!!!!
            {
                // what month are we in
                int month = tCalendar.whatSaturdayIsIt(j).Month;

                // DEBUG Console.WriteLine(month);
                //are we in july?
                if (month == 7)
                {
                    if (((PGY2)AllPgy2s[i]).rolePerMonth[0].DoesLong == false &&
                        ((PGY2)AllPgy2s[i]).rolePerMonth[0].DoesTrainingLong ==
                        false) // if their role doesnt do long calls this month
                    {
                        continue;
                    }
                }
                // then skip over them and dont add the edge because they cant be used

                // are we in august?
                if (month == 8)
                {
                    if (((PGY2)AllPgy2s[i]).rolePerMonth[1].DoesLong == false &&
                        ((PGY2)AllPgy2s[i]).rolePerMonth[1].DoesTrainingLong == false)
                    {
                        continue;
                    }
                }
                // same as above

                saturdayCallGraph.addEdge(2 * i + 1, Sat24hCallAmt * 2 + j,
                    1); //each pgy2 can only train once per day
                //source node, pgy2 index, days pgy1s need to work
            }
        }

        int estPgy2Training
            = (pgy1 + pgy2 - 1) /
              pgy2; // ceiling division to get capacity amount for sink
        for (int i = 0; i < pgy2; i++)
        {
            saturdayCallGraph.addEdge(Sat24hCallAmt * 2 + i, sinkIndex,
                estPgy2Training);
        }

        int flow = saturdayCallGraph.getFlow(sourceIndex, sinkIndex);
        //Console.WriteLine($"The flow is {flow}");
        //Console.WriteLine($"The number of pgy1 is {pgy1}");
        if (flow != 1 * pgy1)
        {
            Console.WriteLine(
                "[ERROR] Not able to make valid assignment based on parameters");
            return false;
        }

        Console.WriteLine(
            "Succesfully created PGY2 saturday training assignment");
        //Console.WriteLine("PGY1 Assignments:");
        for (int i = 0; i < pgy1; i++)
        {
            //Console.WriteLine($" PGY1 #{((PGY1)(AllPgy1s[i])).name}:");
            ArrayList? curList
                = (ArrayList)saturdayCallGraph.adjList[
                    Sat24hCallAmt * 2 + pgy2 + i];
            for (int j = 0; j < curList.Count; j++)
            {
                Edge? currEdge = (Edge)curList[j];
                // check if flow is leaving the current edge
                if (currEdge.flow() > 0)
                {
                    ((PGY1)AllPgy1s[i]).addWorkDay(
                        tCalendar.whatSaturdayIsIt(currEdge.destination / 2));
                }
                //Console.WriteLine($"  Day {tCalendar.whatSaturdayIsIt(currEdge.destination / 2)}");
            }
        }

        //Console.WriteLine("PGY2 Assignments:");
        for (int i = 0; i < pgy2; i++)
        {
            //Console.WriteLine($" PGY2 #{((PGY2)(AllPgy2s[i])).name}");
            ArrayList? curList
                = (ArrayList)saturdayCallGraph.adjList[Sat24hCallAmt * 2 + i];
            for (int j = 0; j < curList.Count; j++)
            {
                Edge? currEdge = (Edge)curList[j];
                // check if the flow is negative
                if (currEdge.flow() < 0)
                {
                    ((PGY2)AllPgy2s[i]).addWorkDay(
                        tCalendar.whatSaturdayIsIt(currEdge.destination / 2));
                }
                //Console.WriteLine($"  Day {tCalendar.whatSaturdayIsIt(currEdge.destination / 2)}");
            }
        }

        // make sure a pgy1 is with a pgy2 on a 12h sunday
        nodeAmt = Sun12hCallAmt * 2 /* split node */ + pgy2 + pgy1 +
                  2 /*source & sink*/; // !!!!node id order!!!!
        sourceIndex = nodeAmt - 2; // -2 for 0 indexing
        sinkIndex = nodeAmt - 1;
        Graph sundaysCallGraph = new(nodeAmt);

        for (int i = 0; i < pgy1; i++)
        {
            sundaysCallGraph.addEdge(sourceIndex, Sun12hCallAmt * 2 + pgy2 + i,
                1);
        }
        //source node, pgy1 index, days pgy1s need to work

        for (int i = 0; i < pgy1; i++)
        {
            for (int j = 0;
                 j < Sun12hCallAmt;
                 j++) // TO DO: THIS IS WHERE VACATION TIME WILL GO!!!!
            {
                // what month are we in
                int month = tCalendar.whatSundayIsIt(j).Month;

                // DEBUG Console.WriteLine(month);
                //are we in july?
                if (month == 7)
                {
                    if (((PGY1)AllPgy1s[i]).rolePerMonth[0].DoesLong == false &&
                        ((PGY1)AllPgy1s[i]).rolePerMonth[0].DoesTrainingLong ==
                        false) // if their role doesnt do long calls this month
                    {
                        continue;
                    }
                }
                // then skip over them and dont add the edge because they cant be used

                // are we in august?
                if (month == 8)
                {
                    if (((PGY1)AllPgy1s[i]).rolePerMonth[1].DoesLong == false &&
                        ((PGY1)AllPgy1s[i]).rolePerMonth[1].DoesTrainingLong == false)
                    {
                        continue;
                    }
                }
                // same as above

                sundaysCallGraph.addEdge(Sun12hCallAmt * 2 + pgy2 + i, 2 * j,
                    1); //each pgy1 has an edge between them and a training day
            }
        }

        for (int i = 0; i < Sun12hCallAmt; i++)
        {
            sundaysCallGraph.addEdge(i * 2, i * 2 + 1,
                1);
        }
        // connecting split nodes. OTHERWISE THERE WOULD BE 0 FLOW IN THE GRAPH

        for (int i = 0; i < pgy2; i++)
        {
            for (int j = 0;
                 j < Sun12hCallAmt;
                 j++) // TO DO: THIS IS WHERE VACATION TIME WILL GO!!!!
            {
                // what month are we in
                int month = tCalendar.whatSundayIsIt(j).Month;

                // DEBUG Console.WriteLine(month);
                //are we in july?
                if (month == 7)
                {
                    if (((PGY2)AllPgy2s[i]).rolePerMonth[0].DoesLong == false &&
                        ((PGY2)AllPgy2s[i]).rolePerMonth[0].DoesTrainingLong ==
                        false) // if their role doesnt do long calls this month
                    {
                        continue;
                    }
                }
                // then skip over them and dont add the edge because they cant be used

                // are we in august?
                if (month == 8)
                {
                    if (((PGY2)AllPgy2s[i]).rolePerMonth[1].DoesLong == false &&
                        ((PGY2)AllPgy2s[i]).rolePerMonth[1].DoesTrainingLong == false)
                    {
                        continue;
                    }
                }
                // same as above

                sundaysCallGraph.addEdge(2 * i + 1, Sun12hCallAmt * 2 + j,
                    1); //each pgy2 can only train once per day
                //source node, pgy2 index, days pgy1s need to work
            }
        }

        //int estPgy2Training = ((pgy1+pgy2-1)/pgy2); // ceiling division to get capacity amount (estimated amount of time pgy2 must train someone) for sink
        for (int i = 0; i < pgy2; i++)
        {
            sundaysCallGraph.addEdge(Sun12hCallAmt * 2 + i, sinkIndex,
                estPgy2Training);
        }

        if (sundaysCallGraph.getFlow(sourceIndex, sinkIndex) != 1 * pgy1)
        {
            Console.WriteLine(
                "[ERROR] Not able to make valid assignment based on parameters");
            return false;
        }

        Console.WriteLine(
            "Succesfully created PGY2 sunday training assignment");
        for (int i = 0; i < pgy1; i++)
        {
            ArrayList? curList
                = (ArrayList)sundaysCallGraph.adjList[
                    Sun12hCallAmt * 2 + pgy2 + i];
            for (int j = 0; j < curList.Count; j++)
            {
                Edge? currEdge = (Edge)curList[j];
                // check if flow is leaving the current edge
                if (currEdge.flow() > 0)
                {
                    ((PGY1)AllPgy1s[i]).addWorkDay(
                        tCalendar.whatSundayIsIt(currEdge.destination / 2));
                }
            }
        }

        for (int i = 0; i < pgy2; i++)
        {
            ArrayList? curList
                = (ArrayList)sundaysCallGraph.adjList[Sun12hCallAmt * 2 + i];
            for (int j = 0; j < curList.Count; j++)
            {
                Edge? currEdge = (Edge)curList[j];
                // check if the flow is negative
                if (currEdge.flow() < 0)
                {
                    ((PGY2)AllPgy2s[i]).addWorkDay(
                        tCalendar.whatSundayIsIt(currEdge.destination / 2));
                }
            }
        }

        // fix the weekends post schedule generation
        FixWeekends1(AllPgy1s);
        FixWeekends2(AllPgy2s);

        // debug print for verification
        Print(AllPgy1s, AllPgy2s, AllPgy3s);

        // savecontent
        Save(AllPgy1s, AllPgy2s, AllPgy3s);
        return true;
    }

    public void Save(ArrayList pgy1s, ArrayList pgy2s, ArrayList pgy3s)
    {
        // save the work days of each resident to a file
        foreach (PGY1 res in pgy1s)
        {
            res.saveWorkDays();
        }

        foreach (PGY2 res in pgy2s)
        {
            res.saveWorkDays();
        }

        foreach (PGY3 res in pgy3s)
        {
            res.saveWorkDays();
        }
    }

    public void Print(ArrayList pgy1s, ArrayList pgy2s, ArrayList pgy3s)
    {
        foreach (PGY1 res in pgy1s)
        {
            _logger.LogDebug("PGY1 {ResName} works:", res.name);
            // print all their work days in sorted order
            ArrayList workedDays = new();
            foreach (DateTime curDay in res.workDaySet())
            {
                workedDays.Add(curDay);
            }

            // sort the worked days array list
            workedDays.Sort();

            foreach (DateTime curDay in workedDays)
            {
                _logger.LogDebug("  {DateTime} {CurDayDayOfWeek}", curDay, curDay.DayOfWeek);
            }
        }

        foreach (PGY2 res in pgy2s)
        {
            _logger.LogDebug("PGY2 {ResName} works:", res.name);
            // print all their work days in sorted order
            ArrayList workedDays = new();
            foreach (DateTime curDay in res.workDaySet())
            {
                workedDays.Add(curDay);
            }

            // sort the worked days array list
            workedDays.Sort();

            foreach (DateTime curDay in workedDays)
            {
                _logger.LogDebug("  {DateTime} {CurDayDayOfWeek}", curDay, curDay.DayOfWeek);
            }
        }

        foreach (PGY3 res in pgy3s)
        {
            _logger.LogDebug("PGY3 {ResName} works:", res.name);
            // print all their work days in sorted order
            ArrayList workedDays = new();
            foreach (DateTime curDay in res.workDaySet())
            {
                workedDays.Add(curDay);
            }

            // sort the worked days array list
            workedDays.Sort();

            foreach (DateTime curDay in workedDays)
            {
                _logger.LogDebug("  {DateTime} {CurDayDayOfWeek}", curDay, curDay.DayOfWeek);
            }
        }
    }

    public bool Part2(int year, ArrayList pgy1s, ArrayList pgy2s)
    {
        Console.WriteLine("part 2: normal schedule (january through june)");
        ArrayList AllPgy1s = pgy1s;
        ArrayList AllPgy2s = pgy2s;

        // store days currently worked by anyone
        HashSet<DateTime> workedDays = new();
        foreach (PGY1 res in AllPgy1s)
        {
            foreach (DateTime curDay in res.workDaySet())
            {
                workedDays.Add(curDay);
            }
        }

        foreach (PGY2 res in AllPgy2s)
        {
            foreach (DateTime curDay in res.workDaySet())
            {
                workedDays.Add(curDay);
            }
        }

        DateTime startDay = new(year + 1, 1, 1);
        DateTime endDay = new(year + 1, 6, 30);

        // compute how many days of each shift type there are
        Dictionary<CallShiftType, int> shiftTypeCount = new();
        for (DateTime curDay = startDay;
             curDay <= endDay;
             curDay = curDay.AddDays(1))
        {
            // check that no one works that day
            if (workedDays.Contains(curDay))
            {
                continue;
            }
            // skip this day if someone already works it

            List<CallShiftType> shifts
                = CallShiftTypeExtensions.GetAllCallShiftTypesForDate(curDay);

            foreach (CallShiftType shiftType in shifts)
            {
                bool wasNew = shiftTypeCount.TryAdd(shiftType, 1);
                if (!wasNew)
                {
                    shiftTypeCount[shiftType]++;
                }
            }
        }

        // randomly assign shifts until one works
        int maxTries = 10;
        bool assigned = false;
        for (int attempt = 0; attempt < maxTries && !assigned; attempt++)
        {
            assigned = RandomAssignment(AllPgy1s, AllPgy2s, startDay, endDay,
                shiftTypeCount, workedDays);
        }

        if (!assigned)
        {
            _logger.LogError(
                "[ERROR] Could not assign random shifts after retries");
            return false;
        }

        // save (and commit)
        Save(AllPgy1s, AllPgy2s,
            new ArrayList()); // PGY3s are not used in part 1
        _logger.LogInformation("Part 2 completed successfully.");
        // Print
        Print(AllPgy1s, AllPgy2s,
            new ArrayList()); // PGY3s are not used in part 1
        return true;
    }

    public bool Part1(int year, ArrayList pgy1s, ArrayList pgy2s)
    {
        _logger.LogInformation("part 1: normal schedule (july through december)");
        int pgy1 = 8;
        int pgy2 = 8;
        ArrayList AllPgy1s = pgy1s;
        ArrayList AllPgy2s = pgy2s;

        for (int i = 0; i < pgy1; i++)
        {
            ((PGY1)AllPgy1s[i]).inTraining = false;

            // assign the training date of the pgy1s based on their last worked day
            ((PGY1)AllPgy1s[i]).lastTrainingDate
                = ((PGY1)AllPgy1s[i]).workDaySet().Max();
        }

        for (int i = 0; i < pgy2; i++)
        {
            ((PGY2)AllPgy2s[i]).inTraining = false;
        }

        // store days currently worked by anyone
        HashSet<DateTime> workedDays = new();
        foreach (PGY1 res in AllPgy1s)
        {
            foreach (DateTime curDay in res.workDaySet())
            {
                workedDays.Add(curDay);
            }
        }

        foreach (PGY2 res in AllPgy2s)
        {
            foreach (DateTime curDay in res.workDaySet())
            {
                workedDays.Add(curDay);
            }
        }

        DateTime startDay = new(year, 7, 7);
        DateTime endDay = new(year, 12, 31);

        // compute how many days of each shift type there are
        Dictionary<CallShiftType, int> shiftTypeCount = new();
        for (DateTime curDay = startDay;
             curDay <= endDay;
             curDay = curDay.AddDays(1))
        {
            // check that no one works that day
            if (workedDays.Contains(curDay))
            {
                continue;
            }
            // skip this day if someone already works it

            List<CallShiftType> shifts
                = CallShiftTypeExtensions.GetAllCallShiftTypesForDate(curDay);

            foreach (CallShiftType shiftType in shifts)
            {
                bool wasNew = shiftTypeCount.TryAdd(shiftType, 1);
                if (!wasNew)
                {
                    shiftTypeCount[shiftType]++;
                }
            }
        }

        // randomly assign shifts until one works
        int maxTries = 10;
        bool assigned = false;
        for (int attempt = 0; attempt < maxTries && !assigned; attempt++)
        {
            assigned = RandomAssignment(AllPgy1s, AllPgy2s, startDay, endDay,
                shiftTypeCount, workedDays);
        }

        if (!assigned)
        {
            _logger.LogError(
                "[ERROR] Could not assign random shifts after retries");
            return false;
        }

        // save (and commit)
        Save(AllPgy1s, AllPgy2s,
            new ArrayList()); // PGY3s are not used in part 1
        _logger.LogInformation("Part 1 completed successfully.");
        // Print
        Print(AllPgy1s, AllPgy2s,
            new ArrayList()); // PGY3s are not used in part 1
        return true;
    }


    public void ComputeWorkTime(ArrayList pgy1s, ArrayList pgy2s,
        int[] pgy1WorkTime, int[] pgy2WorkTime,
        Dictionary<CallShiftType, int>[] pgy1ShiftCount,
        Dictionary<CallShiftType, int>[] pgy2ShiftCount)
    {
        for (int i = 0; i < pgy1s.Count; i++)
        {
            pgy1WorkTime[i] = 0;
            // PGY1? res = (PGY1)pgy1s[i];
            // foreach (DateTime workDay in
            //          res.workDaySet())
            // {
            //     CallShiftType shiftType
            //         = CallShiftTypeExtensions.GetCallShiftTypeForDate(workDay,
            //             1);
            //     pgy1WorkTime[i] += shiftType.GetHours();
            // }

            // add the shift type time to the work time
            foreach (CallShiftType shift in
                     pgy1ShiftCount[i].Keys)
            {
                pgy1WorkTime[i] += shift.GetHours() * pgy1ShiftCount[i][shift];
            }
            // add the assigned shifts to the work time
        }

        for (int i = 0; i < pgy2s.Count; i++)
        {
            pgy2WorkTime[i] = 0;
            // PGY2? res = (PGY2)pgy2s[i];
            // foreach (DateTime workDay in
            //          res.workDaySet())
            // {
            //     CallShiftType shiftType
            //         = CallShiftTypeExtensions.GetCallShiftTypeForDate(workDay,
            //             2);
            //     pgy2WorkTime[i] += shiftType.GetHours();
            // }

            // add the shift type time to the work time
            foreach (CallShiftType shift in
                     pgy2ShiftCount[i].Keys)
            {
                pgy2WorkTime[i] += shift.GetHours() * pgy2ShiftCount[i][shift];
            }
            // add the assigned shifts to the work time
        }
    }

#pragma warning disable IDE0060
    public void InitialShiftAssignment(ArrayList pgy1s, ArrayList pgy2s,
        Dictionary<CallShiftType, int> shiftTypeCount,
        Dictionary<CallShiftType, int>[] pgy1ShiftCount,
        Dictionary<CallShiftType, int>[] pgy2ShiftCount, Random rand,
        Dictionary<CallShiftType, int>[] allowedCallTypes)
    {
        foreach (CallShiftType shift in shiftTypeCount.Keys)
        {
            // Create a random ratio for each shift type
            for (int i = 0; i < shiftTypeCount[shift]; i++)
            // randomly select a pgy1 or pgy2 to assign the shift to
            {
                int? requiredYear = shift.GetRequiredYear();
                int usingYear
                    = requiredYear ?? (rand.NextDouble() < 0.50 ? 1 : 2); // 50% chance to assign to pgy1
                if (usingYear == 1)
                {
                    int pgy1Index = rand.Next(pgy1s.Count);
                    if (!allowedCallTypes[pgy1Index]
                            .ContainsKey(
                                shift)) // check if the pgy1 cannot take this shift
                    {
                        i--;
                        continue;
                    }

                    if (allowedCallTypes[pgy1Index][shift] ==
                        pgy1ShiftCount[pgy1Index]
                            [shift]) // if the pgy1 cannot take any shifts, skip this iteration
                    {
                        i--;
                        continue;
                    }


                    if (!pgy1ShiftCount[pgy1Index].ContainsKey(shift))
                    {
                        pgy1ShiftCount[pgy1Index][shift] = 0;
                    }

                    // initialize the count for this shift type
                    pgy1ShiftCount[pgy1Index][shift]
                        += 1; // increment the count for this shift type
                }
                else // assign to pgy2
                {
                    int pgy2Index = rand.Next(pgy2s.Count);
                    if (!allowedCallTypes[pgy2Index + pgy1s.Count]
                            .ContainsKey(
                                shift)) // check if the pgy2 cannot take this shift
                    {
                        i--;
                        continue;
                    }

                    if (allowedCallTypes[pgy2Index + pgy1s.Count][shift] ==
                        pgy2ShiftCount[pgy2Index]
                            [shift]) // if the pgy2 cannot take any shifts, skip this iteration
                    {
                        i--;
                        continue;
                    }

                    if (!pgy2ShiftCount[pgy2Index].ContainsKey(shift))
                    {
                        pgy2ShiftCount[pgy2Index][shift] = 0;
                    }

                    // initialize the count for this shift type
                    pgy2ShiftCount[pgy2Index][shift] += 1;
                }
            }
        }
    }

#pragma warning restore IDE0060

    public void SwapSomeShiftCount(ArrayList pgy1s, ArrayList pgy2s,
        Dictionary<CallShiftType, int>[] pgy1ShiftCount,
        Dictionary<CallShiftType, int>[] pgy2ShiftCount, Random rand, int[] pgy1WorkTime,
        int[] pgy2WorkTime,
        Dictionary<CallShiftType, int>[] allowedCallTypes)
    {
        // First root the person who worked the most
        bool success = SwapWithGiverRooted(pgy1s, pgy2s, pgy1ShiftCount,
            pgy2ShiftCount, rand, pgy1WorkTime, pgy2WorkTime, allowedCallTypes);

        if (success)
        {
            return;
        }

        success = SwapWithReceiverRooted(pgy1s, pgy2s, pgy1ShiftCount,
            pgy2ShiftCount, rand, pgy1WorkTime, pgy2WorkTime, allowedCallTypes);

        if (!success)
        {
            _logger.LogError("Failed");
        }
    }

    public bool SwapWithGiverRooted(ArrayList pgy1s, ArrayList pgy2s,
        Dictionary<CallShiftType, int>[] pgy1ShiftCount,
        Dictionary<CallShiftType, int>[] pgy2ShiftCount, Random rand,
        int[] pgy1WorkTime,
        int[] pgy2WorkTime,
        Dictionary<CallShiftType, int>[] allowedCallTypes)
    {
        // find the person who worked the most
        int giverIndex = 0;
        int giveHour = -1;
        for (int i = 0; i < pgy1s.Count; i++)
        {
            if (giveHour < pgy1WorkTime[i])
            {
                giveHour = pgy1WorkTime[i];
                giverIndex = i;
            }
        }

        for (int i = 0; i < pgy2s.Count; i++)
        {
            if (giveHour < pgy2WorkTime[i])
            {
                giveHour = pgy2WorkTime[i];
                giverIndex = i + pgy1s.Count;
            }
        }

        // Calculate give-able shifts
        int giverYear = giverIndex < pgy1s.Count ? 1 : 2;
        int normalizedGiverIndex = giverYear == 1 ? giverIndex : giverIndex - pgy1s.Count;
        Dictionary<CallShiftType, int> giverShiftCount =
            giverYear == 1
                ? pgy1ShiftCount[normalizedGiverIndex]
                : pgy2ShiftCount[normalizedGiverIndex];
        List<CallShiftType> giverShiftTypes = giverShiftCount.Where(kvp => kvp.Value > 0).Select(kvp => kvp.Key).ToList();

        // ensure that the giver has worked more than the receiver and they are not the same person
        int count = 0;
        int receiverIndex;
        Dictionary<CallShiftType, int> receiverShiftCount;
        List<CallShiftType> swappableShiftTypes;

        bool needsToCalculate;
        do
        {
            count++;
            // choose a random giving resident and a random receiving resident
            receiverIndex = rand.Next(pgy1s.Count + pgy2s.Count);

            // check that giver and receiver are not the same AND that giver works more than receiver
            int receiveHour = receiverIndex < pgy1s.Count
                ? pgy1WorkTime[receiverIndex]
                : pgy2WorkTime[receiverIndex - pgy1s.Count];

            // make sure they have common shift types
            int receiverYear = receiverIndex < pgy1s.Count ? 1 : 2;
            int normalizedReceiverIndex = receiverYear == 1 ? receiverIndex : receiverIndex - pgy1s.Count;
            receiverShiftCount =
                receiverYear == 1
                    ? pgy1ShiftCount[normalizedReceiverIndex]
                    : pgy2ShiftCount[normalizedReceiverIndex];
            IEnumerable<CallShiftType> receiverShiftTypes = receiverShiftCount
                .Where(kvp => allowedCallTypes[receiverIndex].ContainsKey(kvp.Key)
                              && allowedCallTypes[receiverIndex][kvp.Key] > receiverShiftCount[kvp.Key]
                )
                .Select(kvp => kvp.Key);

            swappableShiftTypes = giverShiftTypes.Intersect(receiverShiftTypes).ToList();

            needsToCalculate = giveHour <= receiveHour + 12
                               || giverIndex == receiverIndex
                               || swappableShiftTypes.Count == 0;
        } while (needsToCalculate && count < 100);

        if (needsToCalculate)
        {
            // Too many attempts
            _logger.LogError("Failed to swap shifts with giver rooted after 100 attempts.");
            return false;
        }

        int shiftIndex = rand.Next(0, swappableShiftTypes.Count);
        CallShiftType shift = swappableShiftTypes[shiftIndex];

        giverShiftCount[shift]--;
        receiverShiftCount[shift]++;

        return true;
    }

    public bool SwapWithReceiverRooted(ArrayList pgy1s, ArrayList pgy2s,
        Dictionary<CallShiftType, int>[] pgy1ShiftCount,
        Dictionary<CallShiftType, int>[] pgy2ShiftCount, Random rand,
        int[] pgy1WorkTime,
        int[] pgy2WorkTime,
        Dictionary<CallShiftType, int>[] allowedCallTypes)
    {
        // find the person who worked the least
        int receiverIndex = 0;
        int receiverHour = -1;
        for (int i = 0; i < pgy1s.Count; i++)
        {
            if (receiverHour > pgy1WorkTime[i])
            {
                receiverHour = pgy1WorkTime[i];
                receiverIndex = i;
            }
        }

        for (int i = 0; i < pgy2s.Count; i++)
        {
            if (receiverHour > pgy2WorkTime[i])
            {
                receiverHour = pgy2WorkTime[i];
                receiverIndex = i + pgy1s.Count;
            }
        }

        // Calculate give-able shifts
        int receiverYear = receiverIndex < pgy1s.Count ? 1 : 2;
        int normalizedReceiverIndex = receiverYear == 1 ? receiverIndex : receiverIndex - pgy1s.Count;
        Dictionary<CallShiftType, int> receiverShiftCount =
            receiverYear == 1
                ? pgy1ShiftCount[normalizedReceiverIndex]
                : pgy2ShiftCount[normalizedReceiverIndex];
        List<CallShiftType> receiverShiftTypes = receiverShiftCount
            .Where(kvp => allowedCallTypes[receiverIndex].ContainsKey(kvp.Key)
                          && allowedCallTypes[receiverIndex][kvp.Key] > receiverShiftCount[kvp.Key]
            )
            .Select(kvp => kvp.Key)
            .ToList();

        // ensure that the giver has worked more than the receiver and they are not the same person
        int count = 0;
        int giverIndex;
        Dictionary<CallShiftType, int> giverShiftCount;
        List<CallShiftType> swappableShiftTypes;

        bool needsToCalculate;
        do
        {
            count++;
            // choose a random giving resident and a random receiving resident
            giverIndex = rand.Next(pgy1s.Count + pgy2s.Count);

            // check that giver and receiver are not the same AND that giver works more than receiver
            int giverHour = giverIndex < pgy1s.Count
                ? pgy1WorkTime[giverIndex]
                : pgy2WorkTime[giverIndex - pgy1s.Count];

            // make sure they have common shift types
            int giverYear = giverIndex < pgy1s.Count ? 1 : 2;
            int normalizedGiverIndex = giverYear == 1 ? giverIndex : giverIndex - pgy1s.Count;
            giverShiftCount =
                giverYear == 1
                    ? pgy1ShiftCount[normalizedGiverIndex]
                    : pgy2ShiftCount[normalizedGiverIndex];
            IEnumerable<CallShiftType> giverShiftTypes = giverShiftCount
                .Where(kvp => kvp.Value > 0).Select(kvp => kvp.Key);

            swappableShiftTypes = giverShiftTypes.Intersect(receiverShiftTypes).ToList();

            needsToCalculate = giverHour <= receiverHour + 12
                               || giverIndex == receiverIndex
                               || swappableShiftTypes.Count == 0;
        } while (needsToCalculate && count < 100);

        if (needsToCalculate)
        {
            // Too many attempts
            _logger.LogError("Failed to swap shifts with giver rooted after 100 attempts.");
            return false;
        }

        int shiftIndex = rand.Next(0, swappableShiftTypes.Count);
        CallShiftType shift = swappableShiftTypes[shiftIndex];

        giverShiftCount[shift]--;
        receiverShiftCount[shift]++;

        return true;
    }

    public bool RandomAssignment(ArrayList pgy1s, ArrayList pgy2s,
        DateTime startDay, DateTime endDay,
        Dictionary<CallShiftType, int> shiftTypeCount, HashSet<DateTime> workedDays)
    {
        //Console.WriteLine("[DEBUG] Attempting random assignment of shifts...");

        // create an random number generator
        int seed = (int)DateTime.Now.Ticks;
        Random rand = new(seed);

        // track how many shifts of each type are given to each resident
        Dictionary<CallShiftType, int>[] pgy1ShiftCount
            = new Dictionary<CallShiftType, int>[pgy1s.Count];
        Dictionary<CallShiftType, int>[] pgy2ShiftCount
            = new Dictionary<CallShiftType, int>[pgy2s.Count];
        Dictionary<CallShiftType, int>[] allowedCallTypes
            = new Dictionary<CallShiftType, int>[pgy1s.Count + pgy2s.Count];

        // initialize data for each resident
        // On Saturdays, PGY1s work 24 hours and PGY2s work 12 hours
        for (int i = 0; i < pgy1s.Count; i++)
        {
            pgy1ShiftCount[i] = new Dictionary<CallShiftType, int>();
            allowedCallTypes[i] = new Dictionary<CallShiftType, int>();
            foreach (CallShiftType shift in CallShiftTypeExtensions
                         .GetAllCallShiftsForYear(1))
            {
                pgy1ShiftCount[i][shift] = 0;
                allowedCallTypes[i][shift] = 0;
            }
        }

        for (int i = 0; i < pgy2s.Count; i++)
        {
            pgy2ShiftCount[i] = new Dictionary<CallShiftType, int>();
            allowedCallTypes[i + pgy1s.Count] = new Dictionary<CallShiftType, int>();
            foreach (CallShiftType shift in CallShiftTypeExtensions
                         .GetAllCallShiftsForYear(2))
            {
                pgy2ShiftCount[i][shift] = 0;
                allowedCallTypes[i + pgy1s.Count][shift] = 0;
            }
        }

        // iterate through the days and determine the maximum number of shifts for each resident
        int numberOfShifts = 0;
        for (DateTime curDay = startDay;
             curDay <= endDay;
             curDay = curDay.AddDays(1))
        {
            numberOfShifts += CallShiftTypeExtensions.GetAllCallShiftTypesForDate(curDay).Count;
            // iterate through each resident and determine if they can work this day
            for (int i = 0; i < pgy1s.Count; i++)
            {
                PGY1? res = (PGY1)pgy1s[i];
                if (res.canWork(curDay) && !workedDays.Contains(curDay))
                {
                    // determine the shift type for this day
                    CallShiftType shiftTypeValue
                        = CallShiftTypeExtensions.GetCallShiftTypeForDate(
                            curDay, 1);

                    allowedCallTypes[i][shiftTypeValue]++;
                }
            }

            for (int i = 0; i < pgy2s.Count; i++)
            {
                PGY2? res = (PGY2)pgy2s[i];
                if (res.canWork(curDay) && !workedDays.Contains(curDay))
                {
                    // determine the shift type for this day
                    CallShiftType shiftTypeValue
                        = CallShiftTypeExtensions.GetCallShiftTypeForDate(
                            curDay, 2);

                    allowedCallTypes[i + pgy1s.Count][shiftTypeValue]++;
                }
            }
        }

        // iterate through all the shift types and assign them to residents randomly
        InitialShiftAssignment(pgy1s, pgy2s, shiftTypeCount,
            pgy1ShiftCount, pgy2ShiftCount, rand,
            allowedCallTypes);

        // only test this assignment a few times adjust if it does not work
        for (int tryCount = 0; tryCount < 10; tryCount++)
        {
            // try a flow if some assignment does not work reduce the allowed call types for the residents based on missing flow
            // compute work time for each resident
            int[] pgy1WorkTime = new int[pgy1s.Count];
            int[] pgy2WorkTime = new int[pgy2s.Count];
            ComputeWorkTime(pgy1s, pgy2s, pgy1WorkTime, pgy2WorkTime,
                pgy1ShiftCount, pgy2ShiftCount);

            // loop until within 24-hour window
            bool inWindow = false;
            int ct2 = 0;
            while (!inWindow)
            {
                ct2++;
                if (ct2 > 100) // prevent infinite loop
                               //Console.WriteLine("Failed to find a valid assignment within 24-hour window after 100 attempts.");
                {
                    return false;
                }

                // exit the method if we cannot swap shifts to reach a valid assignment (within 24-hour window)
                // determine maximum and minimum work time for pgy1 and pgy2
                int max = Math.Max(pgy1WorkTime.Max(), pgy2WorkTime.Max());
                int min = Math.Min(pgy1WorkTime.Min(), pgy2WorkTime.Min());

                // check if the difference is within 12 hours
                if (max - min <= 12)
                {
                    inWindow = true; // if so, we are done
                }
                else
                {
                    // swap a shift count between two residents
                    SwapSomeShiftCount(pgy1s, pgy2s, pgy1ShiftCount,
                        pgy2ShiftCount, rand, pgy1WorkTime, pgy2WorkTime, allowedCallTypes);

                    // recompute work time for each resident
                    ComputeWorkTime(pgy1s, pgy2s, pgy1WorkTime, pgy2WorkTime,
                        pgy1ShiftCount, pgy2ShiftCount);
                }
            }

            // use flow to see if the assignment is possible

            // only need nodes for each resident(pgy1+pgy2) for each shit type(3) and for each day (#days in range)
            int numShiftTypes
                = CallShiftTypeExtensions.GetAllCallShiftTypes().Count;

            // Markers
            int pgy2Start = pgy1s.Count * numShiftTypes;
            int shiftStart = pgy2Start + pgy2s.Count * numShiftTypes;

            int srcIndex = shiftStart + numberOfShifts;
            int snkIndex = srcIndex + 1;

            int totalNodes = snkIndex + 1;
            Graph g = new(totalNodes);

            // make an edge from the source to each residents shift type with capacity based on the chosen shifts to work
            List<CallShiftType> pgy1ShiftTypes
                = CallShiftTypeExtensions.GetAllCallShiftsForYear(1);
            for (int i = 0; i < pgy1s.Count; i++)
            {
                foreach (CallShiftType shiftType in pgy1ShiftTypes)
                {
                    int offset = (int)shiftType;
                    g.addEdge(srcIndex, i * numShiftTypes + offset,
                        pgy1ShiftCount[i][shiftType]);
                }
            }

            List<CallShiftType> pgy2ShiftTypes
                = CallShiftTypeExtensions.GetAllCallShiftsForYear(2);
            for (int i = 0; i < pgy2s.Count; i++)
            {
                foreach (CallShiftType shiftType in pgy2ShiftTypes)
                {
                    int offset = (int)shiftType;
                    g.addEdge(srcIndex, pgy2Start + i * numShiftTypes + offset,
                        pgy2ShiftCount[i][shiftType]);
                }
            }

            ArrayList dayList = new();

            // iterate through each day
            for (DateTime curDay = startDay;
                 curDay <= endDay;
                 curDay = curDay.AddDays(1))
            {
                // skip if worked already
                if (workedDays.Contains(curDay))
                {
                    continue;
                }

                List<CallShiftType> shiftTypes
                    = CallShiftTypeExtensions.GetAllCallShiftTypesForDate(
                        curDay);

                for (int shiftIndex = 0; shiftIndex < shiftTypes.Count; shiftIndex++)
                {
                    dayList.Add(curDay);

                    CallShiftType shift = shiftTypes[shiftIndex];
                    int shiftOffset = (int)shift;
                    int? reqYear = shift.GetRequiredYear();

                    if (reqYear is null or 1)
                    {
                        for (int i = 0; i < pgy1s.Count; i++)
                        {
                            if (((PGY1)pgy1s[i]).canWork(curDay))
                            {
                                g.addEdge(i * numShiftTypes + shiftOffset,
                                    shiftStart + dayList.Count - 1,
                                    1);
                            }
                        }
                    }

                    if (reqYear is null or 2)
                    {
                        for (int i = 0; i < pgy2s.Count; i++)
                        {
                            if (((PGY2)pgy2s[i]).canWork(curDay))
                            {
                                g.addEdge(pgy2Start + i * numShiftTypes + shiftOffset,
                                    shiftStart + dayList.Count - 1,
                                    1);
                            }
                        }
                    }

                    // connect the day to the sink
                    g.addEdge(shiftStart + dayList.Count - 1,
                        snkIndex, 1);
                }
            }

            // run flow
            int flow = g.getFlow(srcIndex, snkIndex);
            _logger.LogDebug($"[DEBUG] flow is {flow} out of {dayList.Count}");

            // if unsuccessful, we need to adjust the allowed call types for the residents
            if (flow != dayList.Count)
            {
                // coder's note: this seems unnecessary at this point, but i already coded it, and it can handle the unhandled shifts
                Dictionary<int, int> unhandledShifts = new();

                // iterate through the edges leaving the source node (to each resident's shift type)
                ArrayList? edgesFromSource = (ArrayList)g.adjList[srcIndex];

                // iterate through the edges to each resident's shift type
                for (int I = 0; I < edgesFromSource.Count; I++)
                {
                    Edge? edge = (Edge)edgesFromSource[I];
                    int residentIndex = edge.destination / numShiftTypes;
                    // Check if the flow is not equal to the assigned shifts
                    if (edge.flow() < edge.originalCap)
                    {
                        // Get the resident's name
                        string residentName = residentIndex < pgy1s.Count
                            ? ((PGY1)pgy1s[residentIndex]).name
                            : ((PGY2)pgy2s[residentIndex - pgy1s.Count]).name;

                        // print the resident who did not handle their shifts
                        _logger.LogError(
                            "[DEBUG] Resident {ResidentName} did not handle " +
                            "their shifts properly. Assigned: {Flow}, Expected: " +
                            "{EdgeOriginalCap}",
                            residentName, edge.flow(), edge.originalCap
                        );
                        return false;
                    }

                    /*
                    // give a different resdient the shifts that were not assigned
                    if (edge.flow() < edge.originalCap)
                    {
                        // get the shift type based on the modulo by 3 of the destination index
                        // there are 3 shift types: 3h, 12h, and 24h
                        int shiftTypeValue = edge.destination % 3 == 0 ? 3 :
                            edge.destination % 3 == 1 ? 12 : 24;

                        // check if the shift type is already in the map
                        if (unhandledShifts.ContainsKey(shiftTypeValue))
                        {
                            unhandledShifts[shiftTypeValue] +=
                                edge.originalCap - edge.flow();
                        }
                        // increment the count for this shift type
                        else
                        {
                            unhandledShifts[shiftTypeValue] =
                                edge.originalCap - edge.flow();
                        }
                        // initialize the count for this shift type

                        // decrement the count for this shift type for the corresponding resident
                        if (residentIndex < pgy1s.Count) // pgy1
                        {
                            if (pgy1ShiftCount[residentIndex]
                                .ContainsKey(shiftTypeValue))
                            {
                                pgy1ShiftCount[residentIndex][shiftTypeValue]
                                    -= edge.originalCap - edge.flow();
                            }

                            // Update the allowed call types for this resident
                            allowedCallTypes[residentIndex][shiftTypeValue]
                                = edge.flow();
                        }
                        else // pgy2
                        {
                            if (pgy2ShiftCount[residentIndex - pgy1s.Count]
                                .ContainsKey(shiftTypeValue))
                            {
                                pgy2ShiftCount[residentIndex - pgy1s.Count][
                                        shiftTypeValue] -=
                                    edge.originalCap - edge.flow();
                            }

                            // Update the allowed call types for this resident
                            allowedCallTypes[residentIndex][shiftTypeValue]
                                = edge.flow();
                        }
                    } */
                }

                /*
                // iterate through the unhandled shifts
                foreach (KeyValuePair<int, int> kvp in unhandledShifts)
                {
                    for (int i = 0; i < kvp.Value; i++)
                    {
                        // find a random resident who can take this shift type
                        int residentIndex
                            = rand.Next(pgy1s.Count + pgy2s.Count);

                        // check if the resident can take this shift type
                        if (residentIndex < pgy1s.Count) // pgy1
                        {
                            if (allowedCallTypes[residentIndex]
                                    .ContainsKey(kvp.Key) &&
                                allowedCallTypes[residentIndex][kvp.Key] >
                                pgy1ShiftCount[residentIndex][kvp.Key])
                            // assign the shift to this resident
                            {
                                pgy1ShiftCount[residentIndex][kvp.Key]++;
                            }
                            else
                            {
                                i--;
                            }
                        }
                        else
                        {
                            if (allowedCallTypes[residentIndex]
                                    .ContainsKey(kvp.Key) &&
                                allowedCallTypes[residentIndex][kvp.Key] >
                                pgy2ShiftCount[residentIndex - pgy1s.Count][
                                    kvp.Key])
                            // assign the shift to this resident
                            {
                                pgy2ShiftCount[residentIndex - pgy1s.Count][
                                    kvp.Key]++;
                            }
                            else
                            {
                                i--;
                            }
                        }
                    }
                }*/


                /*
                // print the unhandled shifts
                Console.WriteLine("[DEBUG] Unhandled shifts:");
                foreach (KeyValuePair<int, int> kvp in unhandledShifts)
                {
                    Console.WriteLine(
                        $"Shift {kvp.Key} hours: {kvp.Value} days");
                } */

                // print the shift counts for each resident
                _logger.LogInformation("Shift counts for each resident:");
                for (int i = 0; i < pgy1s.Count; i++)
                {
                    int totalHours =
                            pgy1ShiftCount[i][CallShiftType.WeekdayShortCall] * 3
                            + pgy1ShiftCount[i][CallShiftType.SaturdayFullCall] * 24
                            + pgy1ShiftCount[i][CallShiftType.SundayHalfCall] * 12;

                    _logger.LogInformation(
                        "PGY1 {I}: Short: {I1}, Saturday Long: {I2}, Sunday: {I3}, Hours: {hours}",
                        ((PGY1)pgy1s[i]).name, pgy1ShiftCount[i][CallShiftType.WeekdayShortCall],
                        pgy1ShiftCount[i][CallShiftType.SaturdayFullCall],
                        pgy1ShiftCount[i][CallShiftType.SundayHalfCall],
                        totalHours
                    );
                }

                for (int i = 0; i < pgy2s.Count; i++)
                {
                    int totalHours =
                        pgy2ShiftCount[i][CallShiftType.WeekdayShortCall] * 3
                        + pgy2ShiftCount[i][CallShiftType.SaturdayHalfCall] * 12
                        + pgy2ShiftCount[i][CallShiftType.SundayHalfCall] * 12;

                    _logger.LogInformation(
                        "PGY2 {I}: Short: {I1}, Saturday Long: {I2}, Sunday: {I3}, Hours: {hours}",
                        ((PGY2)pgy2s[i]).name, pgy2ShiftCount[i][CallShiftType.WeekdayShortCall],
                        pgy2ShiftCount[i][CallShiftType.SaturdayHalfCall],
                        pgy2ShiftCount[i][CallShiftType.SundayHalfCall],
                        totalHours
                    );
                }

                /*Console.WriteLine("[ERROR] Not able to make valid assignment based on parameters");*/
                continue;
            }

            _logger.LogInformation("Shift counts for each resident:");
            for (int i = 0; i < pgy1s.Count; i++)
            {
                int totalHours =
                    pgy1ShiftCount[i][CallShiftType.WeekdayShortCall] * 3
                    + pgy1ShiftCount[i][CallShiftType.SaturdayFullCall] * 24
                    + pgy1ShiftCount[i][CallShiftType.SundayHalfCall] * 12;

                _logger.LogInformation(
                    "PGY1 {I}: Short: {I1}, Saturday Long: {I2}, Sunday: {I3}, Hours: {hours}",
                    ((PGY1)pgy1s[i]).name, pgy1ShiftCount[i][CallShiftType.WeekdayShortCall],
                    pgy1ShiftCount[i][CallShiftType.SaturdayFullCall],
                    pgy1ShiftCount[i][CallShiftType.SundayHalfCall],
                    totalHours
                );
            }

            for (int i = 0; i < pgy2s.Count; i++)
            {
                int totalHours =
                    pgy2ShiftCount[i][CallShiftType.WeekdayShortCall] * 3
                    + pgy2ShiftCount[i][CallShiftType.SaturdayHalfCall] * 12
                    + pgy2ShiftCount[i][CallShiftType.SundayHalfCall] * 12;

                _logger.LogInformation(
                    "PGY2 {I}: Short: {I1}, Saturday Long: {I2}, Sunday: {I3}, Hours: {hours}",
                    ((PGY2)pgy2s[i]).name, pgy2ShiftCount[i][CallShiftType.WeekdayShortCall],
                    pgy2ShiftCount[i][CallShiftType.SaturdayHalfCall],
                    pgy2ShiftCount[i][CallShiftType.SundayHalfCall],
                    totalHours
                );
            }

            // if we reach here, the flow is equal to the number of days, so we can assign the shifts
            Console.WriteLine("[DEBUG] adding worked days");

            // add worked days
            for (int i = 0; i < pgy1s.Count; i++)
            {
                foreach (CallShiftType shift in pgy1ShiftTypes)
                {
                    int offset = (int)shift;
                    ArrayList? curList = (ArrayList)g.adjList[i * numShiftTypes + offset];
                    foreach (Edge edge in curList)
                    {
                        if (edge.flow() >
                            0) // if the flow is positive, this resident works this day
                        {
                            DateTime workDay = (DateTime)dayList[
                                edge.destination - shiftStart];
                            ((PGY1)pgy1s[i]).addWorkDay(workDay);
                        }
                    }
                }
            }

            for (int i = 0; i < pgy2s.Count; i++)
            {
                foreach (CallShiftType shift in pgy2ShiftTypes)
                {
                    int offset = (int)shift;
                    ArrayList? curList
                        = (ArrayList)g.adjList[pgy2Start + i * numShiftTypes + offset];
                    foreach (Edge edge in curList)
                    {
                        if (edge.flow() >
                            0) // if the flow is positive, this resident works this day
                        {
                            DateTime workDay = (DateTime)dayList[
                                edge.destination - shiftStart];
                            ((PGY2)pgy2s[i]).addWorkDay(workDay);
                        }
                    }
                }
            }


            /*Console.WriteLine($"[DEBUG] fixing weekends");*/

            // fix weekends
            FixWeekends1and2(pgy1s, pgy2s);

            return true; // all shifts were assigned correctly
        }

        // if we reach here, we were not able to assign shifts correctly with 10 attempts
        return false;
    }


    // swap 2 residents work days (avoid back to back long calls)
    public static void SwapWorkDays1(PGY1 res1, PGY1 res2, DateTime day1,
        DateTime day2)
    {
        res1.removeWorkDay(day1);
        res2.removeWorkDay(day2);

        res1.addWorkDay(day2);
        res2.addWorkDay(day1);
    }

    public static void SwapWorkDays12(PGY1 res1, PGY2 res2, DateTime day1,
        DateTime day2)
    {
        res1.removeWorkDay(day1);
        res2.removeWorkDay(day2);

        res1.addWorkDay(day2);
        res2.addWorkDay(day1);
    }


    // swap 2 residents work days (avoid back to back long calls)
    public static void SwapWorkDays2(PGY2 res1, PGY2 res2, DateTime day1,
        DateTime day2)
    {
        res1.removeWorkDay(day1);
        res2.removeWorkDay(day2);

        res1.addWorkDay(day2);
        res2.addWorkDay(day1);
    }

    public static void
        FixWeekends1(ArrayList pgy1s) // function to fix pgy1 weekends
    {
        foreach (PGY1 res in pgy1s)
        {
            // get the first and last day the resident works
            DateTime firstDay = res.firstWorkDay();
            DateTime lastDay = res.lastWorkDay();

            // iterate through all the days
            for (DateTime curDay = firstDay;
                 curDay <= lastDay;
                 curDay = curDay.AddDays(1))
            // check if the day is a conflict
            {
                if (res.isWorking(curDay) && !res.canWork(curDay) &&
                    !res.commitedWorkDay(curDay))
                {
                    bool found = false;
                    foreach (PGY1 res2 in pgy1s)
                    {
                        if (res == res2)
                        {
                            continue;
                        }

                        if (!res2.canWork(curDay))
                        {
                            continue;
                        }

                        // Iterate through all the days for resident 2
                        DateTime firstDay2 = res2.firstWorkDay();
                        DateTime lastDay2 = res2.lastWorkDay();

                        for (DateTime otherDay = firstDay2;
                             otherDay <= lastDay2;
                             otherDay = otherDay.AddDays(1))
                        {
                            if (res2.isWorking(otherDay) && shiftType(curDay) ==
                                shiftType(otherDay) &&
                                res.canWork(otherDay) &&
                                !res2.commitedWorkDay(otherDay))
                            {
                                found = true;
                                SwapWorkDays1(res, res2, curDay, otherDay);
                                break;
                            }
                        }

                        if (found)
                        {
                            break;
                        }
                    }

                    if (!found)
                    {
                        Console.WriteLine("Unable to fix weekends for pgy1 :[");
                    }
                }
            }
        }
    }


    public static void
        FixWeekends1and2(ArrayList pgy1s,
            ArrayList pgy2s) // function to fix resident weekends
    {
        foreach (PGY1 res in pgy1s)
        {
            // get the first and last day the resident works
            DateTime firstDay = res.firstWorkDay();
            DateTime lastDay = res.lastWorkDay();

            // iterate through all the days
            for (DateTime curDay = firstDay;
                 curDay <= lastDay;
                 curDay = curDay.AddDays(1))
            // check if the day is a conflict
            {
                if (res.isWorking(curDay) && !res.canWork(curDay) &&
                    !res.commitedWorkDay(curDay))
                {
                    CallShiftType curShiftType
                        = CallShiftTypeExtensions.GetCallShiftTypeForDate(
                            curDay, 1);
                    bool found = false;
                    foreach (PGY1 res2 in pgy1s)
                    {
                        if (res == res2)
                        {
                            continue;
                        }

                        if (!res2.canWork(curDay))
                        {
                            continue;
                        }

                        // Iterate through all the days for resident 2
                        DateTime firstDay2 = res2.firstWorkDay();
                        DateTime lastDay2 = res2.lastWorkDay();

                        for (DateTime otherDay = firstDay2;
                             otherDay <= lastDay2;
                             otherDay = otherDay.AddDays(1))
                        {
                            CallShiftType otherShiftType
                                = CallShiftTypeExtensions.GetCallShiftTypeForDate(
                                    curDay, 1);

                            if (res2.isWorking(otherDay)
                                && curShiftType == otherShiftType
                                && res.canWork(otherDay)
                                && !res2.commitedWorkDay(otherDay))
                            {
                                found = true;
                                SwapWorkDays1(res, res2, curDay, otherDay);
                                break;
                            }
                        }

                        if (found)
                        {
                            break;
                        }
                    }

                    if (!found)
                    {
                        foreach (PGY2 res2 in pgy2s)
                        {
                            if (!res2.canWork(curDay))
                            {
                                continue;
                            }

                            // Iterate through all the days for resident 2
                            DateTime firstDay2 = res2.firstWorkDay();
                            DateTime lastDay2 = res2.lastWorkDay();

                            for (DateTime otherDay = firstDay2;
                                 otherDay <= lastDay2;
                                 otherDay = otherDay.AddDays(1))
                            {
                                CallShiftType otherShiftType
                                    = CallShiftTypeExtensions.GetCallShiftTypeForDate(
                                        curDay, 2);
                                if (res2.isWorking(otherDay)
                                    && curShiftType == otherShiftType
                                    && res.canWork(otherDay)
                                    && !res2.commitedWorkDay(otherDay))
                                {
                                    found = true;
                                    SwapWorkDays12(res, res2, curDay, otherDay);
                                    break;
                                }
                            }

                            if (found)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        foreach (PGY2 res in pgy2s)
        {
            // get the first and last day the resident works
            DateTime firstDay = res.firstWorkDay();
            DateTime lastDay = res.lastWorkDay();

            // iterate through all the days
            for (DateTime curDay = firstDay;
                 curDay <= lastDay;
                 curDay = curDay.AddDays(1))
            // check if the day is a conflict
            {
                if (res.isWorking(curDay) && !res.canWork(curDay) &&
                    !res.commitedWorkDay(curDay))
                {
                    CallShiftType curShiftType
                        = CallShiftTypeExtensions.GetCallShiftTypeForDate(
                            curDay, 2);

                    bool found = false;
                    foreach (PGY2 res2 in pgy2s)
                    {
                        if (res == res2)
                        {
                            continue;
                        }

                        if (!res2.canWork(curDay))
                        {
                            continue;
                        }

                        // Iterate through all the days for resident 2
                        DateTime firstDay2 = res2.firstWorkDay();
                        DateTime lastDay2 = res2.lastWorkDay();

                        for (DateTime otherDay = firstDay2;
                             otherDay <= lastDay2;
                             otherDay = otherDay.AddDays(1))
                        {
                            CallShiftType otherShiftType
                                = CallShiftTypeExtensions.GetCallShiftTypeForDate(
                                    curDay, 2);
                            if (res2.isWorking(otherDay)
                                && curShiftType == otherShiftType
                                && res.canWork(otherDay)
                                && !res2.commitedWorkDay(otherDay))
                            {
                                found = true;
                                SwapWorkDays2(res, res2, curDay, otherDay);
                                break;
                            }
                        }

                        if (found)
                        {
                            break;
                        }
                    }

                    if (!found)
                    {
                        foreach (PGY1 res2 in pgy1s)
                        {
                            if (!res2.canWork(curDay))
                            {
                                continue;
                            }

                            // Iterate through all the days for resident 2
                            DateTime firstDay2 = res2.firstWorkDay();
                            DateTime lastDay2 = res2.lastWorkDay();

                            for (DateTime otherDay = firstDay2;
                                 otherDay <= lastDay2;
                                 otherDay = otherDay.AddDays(1))
                            {
                                CallShiftType otherShiftType
                                    = CallShiftTypeExtensions.GetCallShiftTypeForDate(
                                        curDay, 1);

                                if (res2.isWorking(otherDay)
                                    && curShiftType == otherShiftType
                                    && res.canWork(otherDay)
                                    && !res2.commitedWorkDay(otherDay))
                                {
                                    found = true;
                                    SwapWorkDays12(res2, res, otherDay, curDay);
                                    break;
                                }
                            }

                            if (found)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    public static int shiftType(DateTime curDate)
    {
        if (curDate.DayOfWeek == DayOfWeek.Saturday)
        {
            return 24;
        }

        if (curDate.DayOfWeek == DayOfWeek.Sunday)
        {
            return 12;
        }

        return 3;
    }

    public static void
        FixWeekends2(ArrayList pgy2s) // function to fix pgy2 weekens
    {
        foreach (PGY2 res in pgy2s)
        {
            // get the first and last day the resident works
            DateTime firstDay = res.firstWorkDay();
            DateTime lastDay = res.lastWorkDay();

            // iterate through all the days
            for (DateTime curDay = firstDay;
                 curDay <= lastDay;
                 curDay = curDay.AddDays(1))
            // check if the day is a conflict
            {
                if (res.isWorking(curDay) && !res.canWork(curDay))
                {
                    bool found = false;
                    foreach (PGY2 res2 in pgy2s)
                    {
                        if (res == res2)
                        {
                            continue;
                        }

                        if (!res2.canWork(curDay))
                        {
                            continue;
                        }

                        // Iterate through all the days for resident 2
                        DateTime firstDay2 = res2.firstWorkDay();
                        DateTime lastDay2 = res2.lastWorkDay();

                        for (DateTime otherDay = firstDay2;
                             otherDay <= lastDay2;
                             otherDay = otherDay.AddDays(1))
                        {
                            if (res2.isWorking(otherDay) && shiftType(curDay) ==
                                shiftType(otherDay) &&
                                res.canWork(otherDay))
                            {
                                found = true;
                                SwapWorkDays2(res, res2, curDay, otherDay);
                                break;
                            }
                        }

                        if (found)
                        {
                            break;
                        }
                    }

                    if (!found)
                    {
                        Console.WriteLine("Unable to fix weekends for pgy2 :[");
                    }
                }
            }
        }
    }

    public static List<DatesDTO> GenerateDateRecords(Guid scheduleId,
        List<PGY1> pgy1s, List<PGY2> pgy2s,
        List<PGY3> pgy3s)
    {
        List<DatesDTO> dateRecords = new();

        foreach (PGY1 res in pgy1s)
        {
            foreach (DateTime day in res.workDaySet())
            {
                CallShiftType shiftType
                    = CallShiftTypeExtensions.GetCallShiftTypeForDate(day, 1);
                dateRecords.Add(new DatesDTO
                {
                    DateId = Guid.NewGuid(),
                    ScheduleId = scheduleId,
                    ResidentId
                        = res.id, // Assuming `id` exists in PGY1 class and maps to the backend
                    Date = day,
                    CallType = shiftType.GetDescription(),
                    Hours = shiftType.GetHours(),
                    IsCommitted = true
                });
            }
        }

        foreach (PGY2 res in pgy2s)
        {
            foreach (DateTime day in res.workDaySet())
            {
                CallShiftType shiftType
                    = CallShiftTypeExtensions.GetCallShiftTypeForDate(day, 2);
                dateRecords.Add(new DatesDTO
                {
                    DateId = Guid.NewGuid(),
                    ScheduleId = scheduleId,
                    ResidentId = res.id,
                    Date = day,
                    CallType = shiftType.GetDescription(),
                    Hours = shiftType.GetHours(),
                    IsCommitted = true
                });
            }
        }

        foreach (PGY3 res in pgy3s)
        {
            foreach (DateTime day in res.workDaySet())
            {
                CallShiftType shiftType
                    = CallShiftTypeExtensions.GetCallShiftTypeForDate(day, 3);
                dateRecords.Add(new DatesDTO
                {
                    DateId = Guid.NewGuid(),
                    ScheduleId = scheduleId,
                    ResidentId = res.id,
                    Date = day,
                    CallType = shiftType.GetDescription(),
                    Hours = shiftType.GetHours(),
                    IsCommitted = true
                });
            }
        }

        return dateRecords;
    }

    private static string GetCallType(DateTime date)
    {
        return date.DayOfWeek switch
        {
            DayOfWeek.Saturday => "24h",
            DayOfWeek.Sunday => "12h",
            _ => "Short"
        };
    }
}


// example
/*

*/

// other things to consider : pgy year and where in hospital they work
// month by month, positions in hospital change. which means that we will probably have to schedule month by month (at least in the back end)