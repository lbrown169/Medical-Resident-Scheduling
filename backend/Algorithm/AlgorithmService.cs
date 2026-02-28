using System.Collections;
using System.Text;
using MedicalDemo.Extensions;
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
    public bool Training(int year, List<PGY1DTO> pgy1s, List<PGY2DTO> pgy2s,
        List<PGY3DTO> pgy3s)
    {
        int pgy1 = pgy1s.Count;
        int pgy2 = pgy2s.Count;
        int pgy3 = pgy3s.Count;
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
                    if (pgy1s[i].GetHospitalRoleForMonth(0).DoesShort == false &&
                        pgy1s[i].GetHospitalRoleForMonth(0).DoesTrainingShort ==
                        false) // if their role doesnt do short calls this month
                    {
                        continue;
                    }
                }
                // then skip over them and dont add the edge because they cant be used

                // are we in august?
                if (month == 8)
                {
                    if (pgy1s[i].GetHospitalRoleForMonth(1).DoesShort == false &&
                        pgy1s[i].GetHospitalRoleForMonth(1).DoesTrainingShort == false)
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
            _logger.LogWarning("Not able to make valid assignment based on parameters");
            return false;
        }

        _logger.LogInformation("Successfully created pgy3 weekday training assignment");
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
                    pgy1s[i].AddWorkDay(tCalendar.whatShortDayIsIt(currEdge.destination / 2));
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
                    pgy3s[i].AddWorkDay(tCalendar.whatShortDayIsIt(currEdge.destination / 2));
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
                    if (pgy1s[i].GetHospitalRoleForMonth(0).DoesLong == false &&
                        pgy1s[i].GetHospitalRoleForMonth(0).DoesTrainingLong ==
                         false) // if their role doesnt do long calls this month
                    {
                        continue;
                    }
                }
                // then skip over them and dont add the edge because they cant be used

                // are we in august?
                if (month == 8)
                {
                    if (pgy1s[i].GetHospitalRoleForMonth(1).DoesLong == false &&
                        pgy1s[i].GetHospitalRoleForMonth(1).DoesTrainingLong ==
                        false) // if their role doesnt do long calls this month
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
                    if (pgy2s[i].GetHospitalRoleForMonth(0).DoesLong == false &&
                        pgy2s[i].GetHospitalRoleForMonth(0).DoesTrainingLong ==
                        false) // if their role doesnt do long calls this month
                    {
                        continue;
                    }
                }
                // then skip over them and dont add the edge because they cant be used

                // are we in august?
                if (month == 8)
                {
                    if (pgy2s[i].GetHospitalRoleForMonth(1).DoesLong == false &&
                        pgy2s[i].GetHospitalRoleForMonth(1).DoesTrainingLong ==
                        false) // if their role doesnt do long calls this month
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
            _logger.LogWarning("Not able to make valid assignment based on parameters");
            return false;
        }

        _logger.LogInformation("Successfully created PGY2 saturday training assignment");
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
                    pgy1s[i].AddWorkDay(tCalendar.whatSaturdayIsIt(currEdge.destination / 2));
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
                    pgy2s[i].AddWorkDay(tCalendar.whatSaturdayIsIt(currEdge.destination / 2));
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
                    if (pgy1s[i].GetHospitalRoleForMonth(0).DoesLong == false &&
                        pgy1s[i].GetHospitalRoleForMonth(0).DoesTrainingLong ==
                        false) // if their role doesnt do long calls this month
                    {
                        continue;
                    }
                }
                // then skip over them and dont add the edge because they cant be used

                // are we in august?
                if (month == 8)
                {
                    if (pgy1s[i].GetHospitalRoleForMonth(1).DoesLong == false &&
                        pgy1s[i].GetHospitalRoleForMonth(1).DoesTrainingLong ==
                        false) // if their role doesnt do long calls this month
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
                    if (pgy2s[i].GetHospitalRoleForMonth(0).DoesLong == false &&
                        pgy2s[i].GetHospitalRoleForMonth(0).DoesTrainingLong ==
                        false) // if their role doesnt do long calls this month
                    {
                        continue;
                    }
                }
                // then skip over them and dont add the edge because they cant be used

                // are we in august?
                if (month == 8)
                {
                    if (pgy2s[i].GetHospitalRoleForMonth(1).DoesLong == false &&
                        pgy2s[i].GetHospitalRoleForMonth(1).DoesTrainingLong ==
                        false) // if their role doesnt do long calls this month
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
            _logger.LogWarning("Not able to make valid assignment based on parameters");
            return false;
        }

        _logger.LogInformation("Successfully created PGY2 sunday training assignment");
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
                    pgy1s[i].AddWorkDay(tCalendar.whatSundayIsIt(currEdge.destination / 2));
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
                    pgy2s[i].AddWorkDay(tCalendar.whatSundayIsIt(currEdge.destination / 2));
                }
            }
        }

        // fix the weekends post schedule generation
        FixWeekends1(pgy1s);
        FixWeekends2(pgy2s);

        // debug print for verification
        Print(pgy1s, pgy2s, pgy3s);

        SaveWorkDays(pgy1s, pgy2s, pgy3s);

        return true;
    }

    public void SaveWorkDays(List<PGY1DTO> pgy1s, List<PGY2DTO> pgy2s, List<PGY3DTO> pgy3s)
    {
        foreach (PGY1DTO pgy1 in pgy1s)
        {
            pgy1.SaveWorkDays();
        }

        foreach (PGY2DTO pgy2 in pgy2s)
        {
            pgy2.SaveWorkDays();
        }

        foreach (PGY3DTO pgy3 in pgy3s)
        {
            pgy3.SaveWorkDays();
        }
    }

    public void Print(List<PGY1DTO> pgy1s, List<PGY2DTO> pgy2s, List<PGY3DTO> pgy3s)
    {
        foreach (PGY1DTO res in pgy1s)
        {
            _logger.LogTrace("PGY1 {ResName} works:", res.Name);
            // print all their work days in sorted order
            ArrayList workedDays = new();
            foreach (DateOnly curDay in res.WorkDays)
            {
                workedDays.Add(curDay);
            }

            // sort the worked days array list
            workedDays.Sort();

            foreach (DateOnly curDay in workedDays)
            {
                _logger.LogTrace("  {DateTime} {CurDayDayOfWeek}", curDay, curDay.DayOfWeek);
            }
        }

        foreach (PGY2DTO res in pgy2s)
        {
            _logger.LogTrace("PGY2 {ResName} works:", res.Name);
            // print all their work days in sorted order
            ArrayList workedDays = new();
            foreach (DateOnly curDay in res.WorkDays)
            {
                workedDays.Add(curDay);
            }

            // sort the worked days array list
            workedDays.Sort();

            foreach (DateOnly curDay in workedDays)
            {
                _logger.LogTrace("  {DateTime} {CurDayDayOfWeek}", curDay, curDay.DayOfWeek);
            }
        }

        foreach (PGY3DTO res in pgy3s)
        {
            _logger.LogTrace("PGY3 {ResName} works:", res.Name);
            // print all their work days in sorted order
            ArrayList workedDays = new();
            foreach (DateOnly curDay in res.WorkDays)
            {
                workedDays.Add(curDay);
            }

            // sort the worked days array list
            workedDays.Sort();

            foreach (DateOnly curDay in workedDays)
            {
                _logger.LogTrace("  {DateTime} {CurDayDayOfWeek}", curDay, curDay.DayOfWeek);
            }
        }
    }

    public bool Part2(int year, List<PGY1DTO> pgy1s, List<PGY2DTO> pgy2s, int looseFactor)
    {
        _logger.LogInformation("Generating part 2: normal schedule (january through june)");
        // store days currently worked by anyone
        HashSet<DateOnly> workedDays = [];
        foreach (PGY1DTO res in pgy1s)
        {
            foreach (DateOnly curDay in res.WorkDays)
            {
                workedDays.Add(curDay);
            }
        }

        foreach (PGY2DTO res in pgy2s)
        {
            foreach (DateOnly curDay in res.WorkDays)
            {
                workedDays.Add(curDay);
            }
        }

        DateOnly startDay = new(year, 1, 1);
        DateOnly endDay = new(year, 6, 30);

        // compute how many days of each shift type there are
        Dictionary<CallShiftType, int> shiftTypeCount = new();
        for (DateOnly curDay = startDay;
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
                = CallShiftTypeExtensions.GetAllAlgorithmCallShiftTypesForDate(curDay);

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
            assigned = RandomAssignment(pgy1s, pgy2s, startDay, endDay,
                shiftTypeCount, workedDays, looseFactor);
        }

        if (!assigned)
        {
            _logger.LogWarning(
                "[ERROR] Could not assign random shifts after retries");
            return false;
        }

        _logger.LogInformation("Part 2 completed successfully.");
        // Print
        Print(pgy1s, pgy2s, []); // PGY3s are not used in part 1

        SaveWorkDays(pgy1s, pgy2s, []);
        return true;
    }

    public bool Part1(int year, List<PGY1DTO> pgy1s, List<PGY2DTO> pgy2s, int looseFactor)
    {
        _logger.LogInformation("part 1: normal schedule (july through december)");
        int pgy1 = pgy1s.Count;
        for (int i = 0; i < pgy1; i++)
        {
            pgy1s[i].InTraining = false;

            // assign the training date of the pgy1s based on their last worked day
            pgy1s[i].LastTrainingDate = pgy1s[i].PendingSaveWorkDays.Max();
        }

        // store days currently worked by anyone
        HashSet<DateOnly> workedDays = new();
        foreach (PGY1DTO res in pgy1s)
        {
            foreach (DateOnly curDay in res.PendingSaveWorkDays)
            {
                workedDays.Add(curDay);
            }
        }

        foreach (PGY2DTO res in pgy2s)
        {
            foreach (DateOnly curDay in res.PendingSaveWorkDays)
            {
                workedDays.Add(curDay);
            }
        }

        DateOnly startDay = new(year, 7, 1);
        DateOnly endDay = new(year, 12, 31);

        // compute how many days of each shift type there are
        Dictionary<CallShiftType, int> shiftTypeCount = new();
        for (DateOnly curDay = startDay;
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
                = CallShiftTypeExtensions.GetAllAlgorithmCallShiftTypesForDate(curDay);

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
            assigned = RandomAssignment(pgy1s, pgy2s, startDay, endDay,
                shiftTypeCount, workedDays, looseFactor);
        }

        if (!assigned)
        {
            _logger.LogWarning(
                "[ERROR] Could not assign random shifts after retries");
            return false;
        }

        _logger.LogInformation("Part 1 completed successfully.");
        // Print
        Print(pgy1s, pgy2s, []); // PGY3s are not used in part 1
        SaveWorkDays(pgy1s, pgy2s, []);
        return true;
    }


    public void ComputeWorkTime(List<PGY1DTO> pgy1s, List<PGY2DTO> pgy2s,
        int[] pgy1WorkTime, int[] pgy2WorkTime,
        Dictionary<CallShiftType, int>[] pgy1ShiftCount,
        Dictionary<CallShiftType, int>[] pgy2ShiftCount)
    {
        for (int i = 0; i < pgy1s.Count; i++)
        {
            pgy1WorkTime[i] = 0;

            PGY1DTO res = pgy1s[i];
            foreach (DateOnly workDay in res.WorkDays)
            {
                CallShiftType? shiftType
                    = CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(workDay,
                        1);
                if (shiftType.HasValue)
                {
                    pgy1WorkTime[i] += shiftType.Value.GetHours();
                }
            }

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
            PGY2DTO res = pgy2s[i];
            foreach (DateOnly workDay in res.WorkDays)
            {
                CallShiftType? shiftType
                    = CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(workDay,
                        2);
                if (shiftType.HasValue)
                {
                    pgy2WorkTime[i] += shiftType.Value.GetHours();
                }
            }

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
    public void InitialShiftAssignment(List<PGY1DTO> pgy1s, List<PGY2DTO> pgy2s,
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

    public void SwapSomeShiftCount(List<PGY1DTO> pgy1s, List<PGY2DTO> pgy2s,
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
            _logger.LogWarning("Failed");
        }
    }

    public bool SwapWithGiverRooted(List<PGY1DTO> pgy1s, List<PGY2DTO> pgy2s,
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

        _logger.LogDebug("Giver {index} has {hour} hours", giverIndex, giveHour);

        // Calculate give-able shifts
        int giverYear = giverIndex < pgy1s.Count ? 1 : 2;
        int normalizedGiverIndex = giverYear == 1 ? giverIndex : giverIndex - pgy1s.Count;
        Dictionary<CallShiftType, int> giverShiftCount =
            giverYear == 1
                ? pgy1ShiftCount[normalizedGiverIndex]
                : pgy2ShiftCount[normalizedGiverIndex];
        List<CallShiftType> giverShiftTypes = giverShiftCount.Where(kvp => kvp.Value > 0).Select(kvp => kvp.Key).ToList();

        // Build weighted list of eligible receivers
        List<(int index, int weight)> eligibleReceivers = [];

        for (int i = 0; i < pgy1s.Count + pgy2s.Count; i++)
        {
            if (i == giverIndex)
            {
                continue;
            }

            int receiverHour = i < pgy1s.Count
                ? pgy1WorkTime[i]
                : pgy2WorkTime[i - pgy1s.Count];

            int hourDiff = giveHour - receiverHour;
            if (hourDiff <= 0)
            {
                continue; // must work more than receiver
            }

            int receiverYear = i < pgy1s.Count ? 1 : 2;
            int normalizedIndex = receiverYear == 1 ? i : i - pgy1s.Count;
            Dictionary<CallShiftType, int> shiftCount = receiverYear == 1 ? pgy1ShiftCount[normalizedIndex] : pgy2ShiftCount[normalizedIndex];

            List<CallShiftType> swappable = allowedCallTypes[i]
                .Where(kvp => kvp.Value > shiftCount[kvp.Key])
                .Select(kvp => kvp.Key)
                .Intersect(giverShiftTypes)
                .Where(s =>
                    giveHour - s.GetHours() > receiverHour
                )
                .ToList();

            if (swappable.Count == 0)
            {
                continue;
            }

            eligibleReceivers.Add((i, hourDiff));
        }

        if (eligibleReceivers.Count == 0)
        {
            _logger.LogWarning("No eligible receivers found.");
            return false;
        }

        // Weighted random selection
        int totalWeight = eligibleReceivers.Sum(g => g.weight);
        int roll = rand.Next(totalWeight);
        int cumulative = 0;
        int selectedReceiverIndex = eligibleReceivers[^1].index; // fallback

        foreach ((int index, int weight) in eligibleReceivers)
        {
            cumulative += weight;
            if (roll < cumulative)
            {
                selectedReceiverIndex = index;
                break;
            }
        }

        int finalReceiverYear = selectedReceiverIndex < pgy1s.Count ? 1 : 2;
        int normalizedReceiverIndex = finalReceiverYear == 1 ? selectedReceiverIndex : selectedReceiverIndex - pgy1s.Count;
        int finalReceiverHour = finalReceiverYear == 1
            ? pgy1WorkTime[normalizedReceiverIndex]
            : pgy2WorkTime[normalizedReceiverIndex];
        Dictionary<CallShiftType, int> receiverShiftCount = finalReceiverYear == 1
            ? pgy1ShiftCount[normalizedReceiverIndex]
            : pgy2ShiftCount[normalizedReceiverIndex];
        IEnumerable<CallShiftType> receiverShiftTypes = allowedCallTypes[selectedReceiverIndex].Select(kvp => kvp.Key);
        List<CallShiftType> swappableShiftTypes = giverShiftTypes.Intersect(receiverShiftTypes).ToList();

        int shiftIndex = rand.Next(0, swappableShiftTypes.Count);
        CallShiftType shift = swappableShiftTypes[shiftIndex];

        _logger.LogDebug("Receiver {index} has {hour} hours. Receiving {shift}", selectedReceiverIndex, finalReceiverHour, shift.GetDisplayName());

        giverShiftCount[shift]--;
        receiverShiftCount.TryAdd(shift, 0);
        receiverShiftCount[shift]++;

        return true;
    }

    public bool SwapWithReceiverRooted(List<PGY1DTO> pgy1s, List<PGY2DTO> pgy2s,
        Dictionary<CallShiftType, int>[] pgy1ShiftCount,
        Dictionary<CallShiftType, int>[] pgy2ShiftCount, Random rand,
        int[] pgy1WorkTime,
        int[] pgy2WorkTime,
        Dictionary<CallShiftType, int>[] allowedCallTypes)
    {
        // find the person who worked the least
        int receiverIndex = 0;
        int receiverHour = int.MaxValue;
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
        _logger.LogDebug("Receiver {index} has {hour} hours", receiverIndex, receiverHour);

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

        // Build weighted list of eligible givers
        List<(int index, int weight)> eligibleGivers = [];

        for (int i = 0; i < pgy1s.Count + pgy2s.Count; i++)
        {
            if (i == receiverIndex)
            {
                continue;
            }

            int giverHour = i < pgy1s.Count
                ? pgy1WorkTime[i]
                : pgy2WorkTime[i - pgy1s.Count];

            int hourDiff = giverHour - receiverHour;
            if (hourDiff <= 0)
            {
                continue; // must work more than receiver
            }

            int giverYear = i < pgy1s.Count ? 1 : 2;
            int normalizedIndex = giverYear == 1 ? i : i - pgy1s.Count;
            Dictionary<CallShiftType, int> shiftCount = giverYear == 1 ? pgy1ShiftCount[normalizedIndex] : pgy2ShiftCount[normalizedIndex];

            List<CallShiftType> swappable = shiftCount
                .Where(kvp => kvp.Value > 0)
                .Select(kvp => kvp.Key)
                .Intersect(receiverShiftTypes)
                .Where(s => giverHour - s.GetHours() > receiverHour)
                .ToList();

            if (swappable.Count == 0)
            {
                continue;
            }

            eligibleGivers.Add((i, hourDiff));
        }

        if (eligibleGivers.Count == 0)
        {
            _logger.LogWarning("No eligible givers found.");
            return false;
        }

        // Weighted random selection
        int totalWeight = eligibleGivers.Sum(g => g.weight);
        int roll = rand.Next(totalWeight);
        int cumulative = 0;
        int selectedGiverIndex = eligibleGivers[^1].index; // fallback

        foreach ((int index, int weight) in eligibleGivers)
        {
            cumulative += weight;
            if (roll < cumulative)
            {
                selectedGiverIndex = index;
                break;
            }
        }

        int finalGiverYear = selectedGiverIndex < pgy1s.Count ? 1 : 2;
        int normalizedGiverIndex = finalGiverYear == 1 ? selectedGiverIndex : selectedGiverIndex - pgy1s.Count;
        int finalGiverHour = finalGiverYear == 1
            ? pgy1WorkTime[normalizedGiverIndex]
            : pgy2WorkTime[normalizedGiverIndex];
        Dictionary<CallShiftType, int> giverShiftCount = finalGiverYear == 1
            ? pgy1ShiftCount[normalizedGiverIndex]
            : pgy2ShiftCount[normalizedGiverIndex];
        IEnumerable<CallShiftType> giverShiftTypes = giverShiftCount
            .Where(kvp => kvp.Value > 0).Select(kvp => kvp.Key);
        List<CallShiftType> swappableShiftTypes = giverShiftTypes.Intersect(receiverShiftTypes).ToList();

        int shiftIndex = rand.Next(0, swappableShiftTypes.Count);
        CallShiftType shift = swappableShiftTypes[shiftIndex];

        _logger.LogDebug("Giver {index} has {hour} hours. Giving {shift}", selectedGiverIndex, finalGiverHour, shift.GetDisplayName());

        giverShiftCount[shift]--;
        receiverShiftCount.TryAdd(shift, 0);
        receiverShiftCount[shift]++;

        return true;
    }

    public bool RandomAssignment(List<PGY1DTO> pgy1s, List<PGY2DTO> pgy2s,
        DateOnly startDay, DateOnly endDay,
        Dictionary<CallShiftType, int> shiftTypeCount, HashSet<DateOnly> workedDays, int looseFactor)
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
                         .GetAllAlgorithmCallShiftsForYear(1))
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
                         .GetAllAlgorithmCallShiftsForYear(2))
            {
                pgy2ShiftCount[i][shift] = 0;
                allowedCallTypes[i + pgy1s.Count][shift] = 0;
            }
        }

        // iterate through the days and determine the maximum number of shifts for each resident
        int numberOfShifts = 0;
        for (DateOnly curDay = startDay;
             curDay <= endDay;
             curDay = curDay.AddDays(1))
        {
            numberOfShifts += CallShiftTypeExtensions.GetAllAlgorithmCallShiftTypesForDate(curDay).Count;

            // iterate through each resident and determine if they can work this day
            CallShiftType? pgy1ShiftType
                = CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(
                    curDay, 1);
            if (pgy1ShiftType is not null)
            {
                for (int i = 0; i < pgy1s.Count; i++)
                {
                    PGY1DTO res = pgy1s[i];
                    if (res.CanWork(curDay, pgy1ShiftType.Value.GetLengthType()) && !workedDays.Contains(curDay))
                    {
                        allowedCallTypes[i][pgy1ShiftType.Value]++;
                    }
                }
            }

            CallShiftType? pgy2ShiftType
                = CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(
                    curDay, 2);
            if (pgy2ShiftType is not null)
            {
                for (int i = 0; i < pgy2s.Count; i++)
                {
                    PGY2DTO res = pgy2s[i];
                    if (res.CanWork(curDay, pgy2ShiftType.Value.GetLengthType()) && !workedDays.Contains(curDay))
                    {
                        allowedCallTypes[i + pgy1s.Count][pgy2ShiftType.Value]++;
                    }
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
            _logger.LogDebug("trying");
            // try a flow if some assignment does not work reduce the allowed call types for the residents based on missing flow
            // compute work time for each resident
            int[] pgy1WorkTime = new int[pgy1s.Count];
            int[] pgy2WorkTime = new int[pgy2s.Count];
            ComputeWorkTime(pgy1s, pgy2s, pgy1WorkTime, pgy2WorkTime,
                pgy1ShiftCount, pgy2ShiftCount);

            // loop until within 24-hour window
            bool inWindow = false;
            int ct2 = 0;

            bool rootGiver = true;
            while (!inWindow)
            {
                // exit the method if we cannot swap shifts to reach a valid assignment (within 24-hour window)
                // determine maximum and minimum work time for pgy1 and pgy2
                int max = Math.Max(pgy1WorkTime.Max(), pgy2WorkTime.Max());
                int min = Math.Min(pgy1WorkTime.Min(), pgy2WorkTime.Min());

                _logger.LogDebug("Max: {max}, Min: {min}, About to root giver? {swapGiver}", max, min, rootGiver);

                ct2++;
                if (ct2 > 100) // prevent infinite loop
                               //Console.WriteLine("Failed to find a valid assignment within 24-hour window after 100 attempts.");
                {
                    return false;
                }

                // check if within range
                // if algorithm has run more than 50 times, only concern ourselves with matching up hours in each part of schedule
                if (max - min <= looseFactor)
                {
                    inWindow = true; // if so, we are done
                }
                else
                {
                    if (rootGiver)
                    {
                        SwapWithGiverRooted(pgy1s, pgy2s, pgy1ShiftCount,
                            pgy2ShiftCount, rand, pgy1WorkTime, pgy2WorkTime, allowedCallTypes);
                    }
                    else
                    {
                        SwapWithReceiverRooted(pgy1s, pgy2s, pgy1ShiftCount,
                            pgy2ShiftCount, rand, pgy1WorkTime, pgy2WorkTime, allowedCallTypes);
                    }

                    rootGiver = !rootGiver;

                    // swap a shift count between two residents
                    // SwapSomeShiftCount(pgy1s, pgy2s, pgy1ShiftCount,
                    //     pgy2ShiftCount, rand, pgy1WorkTime, pgy2WorkTime, allowedCallTypes);

                    // recompute work time for each resident
                    ComputeWorkTime(pgy1s, pgy2s, pgy1WorkTime, pgy2WorkTime,
                        pgy1ShiftCount, pgy2ShiftCount);
                }
            }

            // use flow to see if the assignment is possible

            // only need nodes for each resident(pgy1+pgy2) for each shit type(3) and for each day (#days in range)
            int numShiftTypes
                = CallShiftTypeExtensions.GetAllAlgorithmCallShiftTypes().Count;

            // Markers
            int pgy2Start = pgy1s.Count * numShiftTypes;
            int shiftStart = pgy2Start + pgy2s.Count * numShiftTypes;

            int srcIndex = shiftStart + numberOfShifts;
            int snkIndex = srcIndex + 1;

            int totalNodes = snkIndex + 1;
            Graph g = new(totalNodes);

            // make an edge from the source to each residents shift type with capacity based on the chosen shifts to work
            List<CallShiftType> pgy1ShiftTypes
                = CallShiftTypeExtensions.GetAllAlgorithmCallShiftsForYear(1);
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
                = CallShiftTypeExtensions.GetAllAlgorithmCallShiftsForYear(2);
            for (int i = 0; i < pgy2s.Count; i++)
            {
                foreach (CallShiftType shiftType in pgy2ShiftTypes)
                {
                    int offset = (int)shiftType;
                    g.addEdge(srcIndex, pgy2Start + i * numShiftTypes + offset,
                        pgy2ShiftCount[i][shiftType]);
                }
            }

            List<DateOnly> dayList = [];

            // iterate through each day
            for (DateOnly curDay = startDay;
                 curDay <= endDay;
                 curDay = curDay.AddDays(1))
            {
                // skip if worked already
                if (workedDays.Contains(curDay))
                {
                    continue;
                }

                List<CallShiftType> shiftTypes
                    = CallShiftTypeExtensions.GetAllAlgorithmCallShiftTypesForDate(
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
                            if (pgy1s[i].CanWork(curDay, shift.GetLengthType()))
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
                            if (pgy2s[i].CanWork(curDay, shift.GetLengthType()))
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
                _ = new                // coder's note: this seems unnecessary at this point, but i already coded it, and it can handle the unhandled shifts
                Dictionary<int, int>();

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
                            ? pgy1s[residentIndex].Name
                            : pgy2s[residentIndex - pgy1s.Count].Name;

                        // print the resident who did not handle their shifts
                        _logger.LogWarning(
                            "[DEBUG] Resident {ResidentName} did not handle " +
                            "their shifts properly. Assigned: {Flow}, Expected: " +
                            "{EdgeOriginalCap}",
                            residentName, edge.flow(), edge.originalCap
                        );
                        return false;
                    }
                }

                // print the shift counts for each resident
                _logger.LogDebug("Shift counts for each resident:");
                for (int i = 0; i < pgy1s.Count; i++)
                {
                    StringBuilder sb = new($"PGY1 {pgy1s[i].Name}: ");
                    int hours = 0;
                    foreach (KeyValuePair<CallShiftType, int> kvp in pgy1ShiftCount[i])
                    {
                        sb.Append($"{kvp.Value} {kvp.Key.GetDisplayName()}, ");
                        hours += kvp.Key.GetHours() * kvp.Value;
                    }

                    sb.Append($"Hours: {hours}");
                    _logger.LogDebug(sb.ToString());
                }

                for (int i = 0; i < pgy2s.Count; i++)
                {
                    StringBuilder sb = new($"PGY2 {pgy2s[i].Name}: ");
                    int hours = 0;
                    foreach (KeyValuePair<CallShiftType, int> kvp in pgy2ShiftCount[i])
                    {
                        sb.Append($"{kvp.Value} {kvp.Key.GetDisplayName()}, ");
                        hours += kvp.Key.GetHours() * kvp.Value;
                    }

                    sb.Append($"Hours: {hours}");
                    _logger.LogDebug(sb.ToString());
                }

                /*Console.WriteLine("[ERROR] Not able to make valid assignment based on parameters");*/
                continue;
            }

            _logger.LogDebug("Shift counts for each resident:");
            for (int i = 0; i < pgy1s.Count; i++)
            {
                StringBuilder sb = new($"PGY1 {pgy1s[i].Name}: ");
                int hours = 0;
                foreach (KeyValuePair<CallShiftType, int> kvp in pgy1ShiftCount[i])
                {
                    sb.Append($"{kvp.Value} {kvp.Key.GetDisplayName()}, ");
                    hours += kvp.Key.GetHours() * kvp.Value;
                }

                sb.Append($"Hours: {hours}");
                _logger.LogDebug(sb.ToString());
            }

            for (int i = 0; i < pgy2s.Count; i++)
            {
                StringBuilder sb = new($"PGY2 {pgy2s[i].Name}: ");
                int hours = 0;
                foreach (KeyValuePair<CallShiftType, int> kvp in pgy2ShiftCount[i])
                {
                    sb.Append($"{kvp.Value} {kvp.Key.GetDisplayName()}, ");
                    hours += kvp.Key.GetHours() * kvp.Value;
                }

                sb.Append($"Hours: {hours}");
                _logger.LogDebug(sb.ToString());
            }

            // if we reach here, the flow is equal to the number of days, so we can assign the shifts
            _logger.LogTrace("Adding worked days");

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
                            DateOnly workDay = dayList[
                                edge.destination - shiftStart];
                            pgy1s[i].AddWorkDay(workDay);
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
                            DateOnly workDay = dayList[
                                edge.destination - shiftStart];
                            pgy2s[i].AddWorkDay(workDay);
                        }
                    }
                }
            }


            /*Console.WriteLine($"[DEBUG] fixing weekends");*/

            // fix weekends
            if (FixWeekends1and2(pgy1s, pgy2s))
            {
                return true; // all shifts were assigned correctly
            }

            foreach (PGY1DTO pgy1 in pgy1s)
            {
                pgy1.WorkDays.Clear();
            }

            foreach (PGY2DTO pgy2 in pgy2s)
            {
                pgy2.WorkDays.Clear();
            }
        }

        // if we reach here, we were not able to assign shifts correctly with 10 attempts
        return false;
    }


    // swap 2 residents work days (avoid back to back long calls)
    public static void SwapWorkDays1(PGY1DTO res1, PGY1DTO res2, DateOnly day1,
        DateOnly day2)
    {
        res1.RemoveWorkDay(day1);
        res2.RemoveWorkDay(day2);

        res1.AddWorkDay(day2);
        res2.AddWorkDay(day1);
    }

    public static void SwapWorkDays12(PGY1DTO res1, PGY2DTO res2, DateOnly day1,
        DateOnly day2)
    {
        res1.RemoveWorkDay(day1);
        res2.RemoveWorkDay(day2);

        res1.AddWorkDay(day2);
        res2.AddWorkDay(day1);
    }


    // swap 2 residents work days (avoid back to back long calls)
    public static void SwapWorkDays2(PGY2DTO res1, PGY2DTO res2, DateOnly day1,
        DateOnly day2)
    {
        res1.RemoveWorkDay(day1);
        res2.RemoveWorkDay(day2);

        res1.AddWorkDay(day2);
        res2.AddWorkDay(day1);
    }

    public bool
        FixWeekends1(List<PGY1DTO> pgy1s) // function to fix pgy1 weekends
    {
        bool didFail = false;
        foreach (PGY1DTO res in pgy1s)
        {
            // get the first and last day the resident works
            DateOnly firstDay = res.FirstWorkDay();
            DateOnly lastDay = res.LastWorkDay();

            // iterate through all the days
            for (DateOnly curDay = firstDay;
                 curDay <= lastDay;
                 curDay = curDay.AddDays(1))
            // check if the day is a conflict
            {
                CallShiftType? curShiftType
                    = CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(curDay, 1);

                if (!curShiftType.HasValue)
                {
                    // no PGY1 shift types, none must be assigned
                    continue;
                }

                if (res.IsWorking(curDay) && !res.CanWork(curDay, curShiftType.Value.GetLengthType()))
                {
                    bool found = false;
                    foreach (PGY1DTO res2 in pgy1s)
                    {
                        if (res == res2)
                        {
                            continue;
                        }

                        if (!res2.CanAddWorkDay(curDay, curShiftType.Value.GetLengthType()))
                        {
                            continue;
                        }

                        // Iterate through all the days for resident 2
                        DateOnly firstDay2 = res2.FirstWorkDay();
                        DateOnly lastDay2 = res2.LastWorkDay();

                        for (DateOnly otherDay = firstDay2;
                             otherDay <= lastDay2;
                             otherDay = otherDay.AddDays(1))
                        {
                            CallShiftType? otherShiftType
                                = CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(otherDay, 1);

                            if (!otherShiftType.HasValue)
                            {
                                continue;
                            }

                            if (res2.IsWorking(otherDay)
                                && curShiftType == otherShiftType
                                && res.CanAddWorkDay(otherDay, otherShiftType.Value.GetLengthType()))
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
                        _logger.LogWarning("Unable to fix {curDay} for PGY{pgy} {name} ({id})", curDay, "1", res.Name, res.ResidentId);
                        didFail = true;
                    }
                }
            }
        }

        return !didFail;
    }


    public bool
        FixWeekends1and2(List<PGY1DTO> pgy1s,
            List<PGY2DTO> pgy2s) // function to fix resident weekends
    {
        bool didFail = false;
        foreach (PGY1DTO res in pgy1s)
        {
            // get the first and last day the resident works
            DateOnly firstDay = res.FirstWorkDay();
            DateOnly lastDay = res.LastWorkDay();

            // iterate through all the days
            for (DateOnly curDay = firstDay;
                 curDay <= lastDay;
                 curDay = curDay.AddDays(1))
            // check if the day is a conflict
            {
                CallShiftType? curShiftType
                    = CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(
                        curDay, 1);

                if (!curShiftType.HasValue)
                {
                    // no pgy1 shift for this date
                    continue;
                }

                if (res.IsWorking(curDay) && !res.CanWork(curDay, curShiftType.Value.GetLengthType()))
                {
                    // They have this shift, it must be valid for their year

                    bool found = false;

                    foreach (PGY1DTO res2 in pgy1s)
                    {
                        if (res == res2)
                        {
                            continue;
                        }

                        if (!res2.CanAddWorkDay(curDay, curShiftType.Value.GetLengthType()))
                        {
                            continue;
                        }

                        // Iterate through all the days for resident 2
                        DateOnly firstDay2 = res2.FirstWorkDay();
                        DateOnly lastDay2 = res2.LastWorkDay();

                        for (DateOnly otherDay = firstDay2;
                             otherDay <= lastDay2;
                             otherDay = otherDay.AddDays(1))
                        {
                            CallShiftType? otherShiftType
                                = CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(
                                    otherDay, 1);

                            if (!otherShiftType.HasValue)
                            {
                                continue;
                            }

                            if (res2.IsWorking(otherDay)
                                && curShiftType == otherShiftType
                                && res.CanAddWorkDay(otherDay, otherShiftType.Value.GetLengthType()))
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
                        CallShiftType? curShiftTypeForPgy2
                            = CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(
                                curDay, 2);

                        if (curShiftType == curShiftTypeForPgy2)
                        {
                            foreach (PGY2DTO res2 in pgy2s)
                            {
                                if (!res2.CanAddWorkDay(curDay, curShiftTypeForPgy2.Value.GetLengthType()))
                                {
                                    continue;
                                }

                                // Iterate through all the days for resident 2
                                DateOnly firstDay2 = res2.FirstWorkDay();
                                DateOnly lastDay2 = res2.LastWorkDay();

                                for (DateOnly otherDay = firstDay2;
                                     otherDay <= lastDay2;
                                     otherDay = otherDay.AddDays(1))
                                {
                                    CallShiftType? otherShiftType
                                        = CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(
                                            otherDay, 2);

                                    if (!otherShiftType.HasValue)
                                    {
                                        continue;
                                    }

                                    if (res2.IsWorking(otherDay)
                                        && curShiftType == otherShiftType
                                        && res.CanAddWorkDay(otherDay, otherShiftType.Value.GetLengthType()))
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

                    if (!found)
                    {
                        _logger.LogWarning("Unable to fix {curDay} for PGY{pgy} {name} ({id})", curDay, "1", res.Name, res.ResidentId);
                    }
                }
            }
        }

        foreach (PGY2DTO res in pgy2s)
        {
            // get the first and last day the resident works
            DateOnly firstDay = res.FirstWorkDay();
            DateOnly lastDay = res.LastWorkDay();

            // iterate through all the days
            for (DateOnly curDay = firstDay;
                 curDay <= lastDay;
                 curDay = curDay.AddDays(1))
            // check if the day is a conflict
            {
                CallShiftType? curShiftType
                    = CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(
                        curDay, 2);

                if (!curShiftType.HasValue)
                {
                    // no pgy2 shifts for this day
                    continue;
                }

                if (res.IsWorking(curDay) && !res.CanWork(curDay, curShiftType.Value.GetLengthType()))
                {

                    bool found = false;
                    foreach (PGY2DTO res2 in pgy2s)
                    {
                        if (res == res2)
                        {
                            continue;
                        }

                        if (!res2.CanAddWorkDay(curDay, curShiftType.Value.GetLengthType()))
                        {
                            continue;
                        }

                        // Iterate through all the days for resident 2
                        DateOnly firstDay2 = res2.FirstWorkDay();
                        DateOnly lastDay2 = res2.LastWorkDay();

                        for (DateOnly otherDay = firstDay2;
                             otherDay <= lastDay2;
                             otherDay = otherDay.AddDays(1))
                        {
                            CallShiftType? otherShiftType
                                = CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(
                                    otherDay, 2);

                            if (!otherShiftType.HasValue)
                            {
                                continue;
                            }

                            if (res2.IsWorking(otherDay)
                                && curShiftType == otherShiftType
                                && res.CanAddWorkDay(otherDay, otherShiftType.Value.GetLengthType()))
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
                        CallShiftType? curShiftTypeForPgy1
                            = CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(
                                curDay, 1);

                        if (curShiftType == curShiftTypeForPgy1)
                        {
                            foreach (PGY1DTO res2 in pgy1s)
                            {
                                if (!res2.CanAddWorkDay(curDay, curShiftTypeForPgy1.Value.GetLengthType()))
                                {
                                    continue;
                                }

                                // Iterate through all the days for resident 2
                                DateOnly firstDay2 = res2.FirstWorkDay();
                                DateOnly lastDay2 = res2.LastWorkDay();

                                for (DateOnly otherDay = firstDay2;
                                     otherDay <= lastDay2;
                                     otherDay = otherDay.AddDays(1))
                                {
                                    // They have this shift, it must be valid for their year
                                    CallShiftType otherShiftType
                                        = CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(
                                            otherDay, 1)!.Value;

                                    if (res2.IsWorking(otherDay)
                                        && curShiftType == otherShiftType
                                        && res.CanAddWorkDay(otherDay, otherShiftType.GetLengthType()))
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

                    if (!found)
                    {
                        _logger.LogWarning("Unable to fix {curDay} for PGY{pgy} {name} ({id})", curDay, "2", res.Name, res.ResidentId);
                        didFail = true;
                    }
                }
            }
        }

        return !didFail;
    }

    public bool
        FixWeekends2(List<PGY2DTO> pgy2s) // function to fix pgy2 weekens
    {
        bool didFail = false;
        foreach (PGY2DTO res in pgy2s)
        {
            // get the first and last day the resident works
            DateOnly firstDay = res.FirstWorkDay();
            DateOnly lastDay = res.LastWorkDay();

            // iterate through all the days
            for (DateOnly curDay = firstDay;
                 curDay <= lastDay;
                 curDay = curDay.AddDays(1))
            // check if the day is a conflict
            {
                CallShiftType? curShiftType
                    = CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(curDay, 2);

                if (!curShiftType.HasValue)
                {
                    // no pgy2 shifts for this date
                    continue;
                }

                if (res.IsWorking(curDay) && !res.CanWork(curDay, curShiftType.Value.GetLengthType()))
                {
                    bool found = false;
                    foreach (PGY2DTO res2 in pgy2s)
                    {
                        if (res == res2)
                        {
                            continue;
                        }

                        if (!res2.CanAddWorkDay(curDay, curShiftType.Value.GetLengthType()))
                        {
                            continue;
                        }

                        // Iterate through all the days for resident 2
                        DateOnly firstDay2 = res2.FirstWorkDay();
                        DateOnly lastDay2 = res2.LastWorkDay();

                        for (DateOnly otherDay = firstDay2;
                             otherDay <= lastDay2;
                             otherDay = otherDay.AddDays(1))
                        {
                            // they're working this, so we know it;s valid
                            CallShiftType otherShiftType
                                = CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(otherDay,
                                    2)!.Value;
                            if (res2.IsWorking(otherDay)
                                && curShiftType == otherShiftType
                                && res.CanAddWorkDay(otherDay, otherShiftType.GetLengthType()))
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
                        _logger.LogWarning("Unable to fix {curDay} for PGY{pgy} {name} ({id})", curDay, "2", res.Name, res.ResidentId);
                        didFail = true;
                    }
                }
            }
        }

        return !didFail;
    }

    public static List<DatesDTO> GenerateDateRecords(Guid scheduleId,
        List<PGY1DTO> pgy1s, List<PGY2DTO> pgy2s,
        List<PGY3DTO> pgy3s)
    {
        List<DatesDTO> dateRecords = [];

        foreach (PGY1DTO res in pgy1s)
        {
            foreach (DateOnly day in res.PendingSaveWorkDays)
            {
                // They have this shift, it must be valid for their year
                CallShiftType shiftType
                    = CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(day, 1)!.Value;
                dateRecords.Add(new DatesDTO
                {
                    DateId = Guid.NewGuid(),
                    ScheduleId = scheduleId,
                    ResidentId
                        = res.ResidentId, // Assuming `id` exists in PGY1 class and maps to the backend
                    Date = day,
                    CallType = shiftType,
                    Hours = shiftType.GetHours(),
                    IsCommitted = true
                });
            }
        }

        foreach (PGY2DTO res in pgy2s)
        {
            foreach (DateOnly day in res.PendingSaveWorkDays)
            {
                // They have this shift, it must be valid for their year
                CallShiftType shiftType
                    = CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(day, 2)!.Value;
                dateRecords.Add(new DatesDTO
                {
                    DateId = Guid.NewGuid(),
                    ScheduleId = scheduleId,
                    ResidentId = res.ResidentId,
                    Date = day,
                    CallType = shiftType,
                    Hours = shiftType.GetHours(),
                    IsCommitted = true
                });
            }
        }

        foreach (PGY3DTO res in pgy3s)
        {
            foreach (DateOnly day in res.PendingSaveWorkDays)
            {
                // They have this shift, it must be valid for their year
                CallShiftType shiftType
                    = CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(day, 3)!.Value;
                dateRecords.Add(new DatesDTO
                {
                    DateId = Guid.NewGuid(),
                    ScheduleId = scheduleId,
                    ResidentId = res.ResidentId,
                    Date = day,
                    CallType = shiftType,
                    Hours = shiftType.GetHours(),
                    IsCommitted = true
                });
            }
        }

        return dateRecords;
    }
}


// example
/*

*/

// other things to consider : pgy year and where in hospital they work
// month by month, positions in hospital change. which means that we will probably have to schedule month by month (at least in the back end)