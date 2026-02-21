

using MedicalDemo.Enums;
using MedicalDemo.Models.Entities;

namespace RotationScheduleGenerator.Algorithm;

public class PGY4RotationScheduleGenerator
{
    private readonly int seed;
    private readonly Random seededRandom;

    private readonly AlgorithmRotationPrefRequest[] requests;
    private readonly IConstraintRule[] constraints;

    private readonly Dictionary<Resident, PGY4RotationTypeEnum?[]> rotationSchedule = []; // Generated schedule
    private readonly int totalMonths = 12;

    private readonly IConstraintRule[] requiredConstraints;

    // Parameters
    private const double PER_MONTH_DIVERSITY_PENALTY = 1; // Penalty score applied when a rotation is used more often for all the months
    private const double SINGLE_MONTH_DIVERSITY_PENALTY = 18; // Penalty score applied when a rotation is used more often for the current month
    private const double REQUIRED_ROTATION_URGENCY_MULTIPLIER = 12; // Score multiplier when a required rotation needs to be assigned
    private const double CLUSTER_PENALTY = 1; // Penalty score applied when the same rotation is applied in the previous month

    // Randomization Parameters
    private const double CHANCE_OF_PRIO_SWAPPING = 0.1;

    // Fitness Parameters
    private const double PRIORITY_GAINED_POINT = 4; // Max score to gain when first priority is picked
    private const double PER_MONTH_DIVERSITY_GAINED_POINT = 2; // Score gained for each unique type of rotation types per month
    private const double SINGLE_MONTH_DIVERSITY_GAINED_POINT = 4; // Score gained for each unique type of rotation types in a single month
    private const double NON_CLUSTER_GAINED_POINT = 0.5; // Score gained when 2 consecutive months do not share the same rotation type
    private const double SINGLE_INPATIENT_CONSULT_DIVERSITY_GAINED_POINT = 100; // Score gained when 2 consecutive months do not share the same rotation type

    private double priorityGainedScore = 0;
    private double perMonthDiversityGainedScore = 0;
    private double singleMonthGainedScore = 0;
    private double nonClusterGainedScore = 0;
    private double singleMonthInpatientConsultDiversitScore = 0;
    private double scheduleFitness = 0;

    public double ScheduleFitness => scheduleFitness;
    public Dictionary<Resident, PGY4RotationTypeEnum?[]> RotationSchedule => rotationSchedule;

    public PGY4RotationScheduleGenerator(
        AlgorithmRotationPrefRequest[] requests,
        IConstraintRule[] constraints,
        int seed
    )
    {
        // Seeding
        this.seed = seed;
        seededRandom = new(seed);

        this.requests = RandomizeRequestOrder(requests);
        this.constraints = constraints;
        requiredConstraints = new IConstraintRule[this.constraints.Length];

        // Initialize rotation schedule
        foreach (AlgorithmRotationPrefRequest request in this.requests)
        {
            rotationSchedule.Add(request.Requester, new PGY4RotationTypeEnum?[totalMonths]);
        }
    }

    private AlgorithmRotationPrefRequest[] RandomizeRequestOrder(AlgorithmRotationPrefRequest[] requests)
    {
        List<int> randNums = [];
        for (int i = 0; i < requests.Length; i++)
        {
            randNums.Add(i);
        }

        List<AlgorithmRotationPrefRequest> randomizedRequests = [];

        for (int i = 0; i < requests.Length; i++)
        {
            int randomIndex = seededRandom.Next(0, randNums.Count);
            int randomIndexValue = randNums[randomIndex];
            randNums.RemoveAt(randomIndex);
            randomizedRequests.Add(requests[randomIndexValue]);
        }

        return [.. randomizedRequests];
    }

    public void GenerateSchedule()
    {
        if (AssignNext(0, 0))
        {
            EvaluatePerformance();

            scheduleFitness =
                priorityGainedScore
                + perMonthDiversityGainedScore
                + singleMonthGainedScore
                + nonClusterGainedScore
                + singleMonthInpatientConsultDiversitScore;
        }
        else
        {
            Console.WriteLine($"Schedule with seed: {seed} failed to generate!");
        }
    }

    private bool AssignNext(int requestIndex, int month)
    {
        // Check if schedule is complete
        if (month == totalMonths)
        {
            // Has completed the last month, schedule is complete
            return true;
        }

        if (IsPreviousRotationNull(requestIndex, month))
        {
            return false;
        }

        AlgorithmRotationPrefRequest request = requests[requestIndex];
        Resident resident = request.Requester;

        PGY4RotationTypeEnum[] availableRotations = GetPreferredRotationTypeEnum(
            request,
            month,
            requiredConstraints,
            out int numRequiredConstraints
        );

        if (availableRotations.Length == 0)
        {
            JumpByRequiredConstraints(numRequiredConstraints, requestIndex, month);
            return false;
        }

        // Try every possible rotation types
        foreach (PGY4RotationTypeEnum rotationType in availableRotations)
        {
            if (IsPreviousRotationNull(requestIndex, month))
            {
                return false;
            }

            if (IsValidRotationTypeAssignment(requestIndex, month, rotationType))
            {
                // Assign
                rotationSchedule[resident][month] = rotationType;

                // Assignment works, move onto next

                if (requestIndex == requests.Length - 1)
                {
                    // Finished all assignments in this month for all residents, move onto next month

                    if (AssignNext(0, month + 1))
                    {
                        return true;
                    }
                }
                else
                {
                    // Move onto next resident in current month
                    if (AssignNext(requestIndex + 1, month))
                    {
                        return true;
                    }
                }

                // Undo assignment
                rotationSchedule[resident][month] = null;
            }
        }

        return false;
    }

    private void JumpByRequiredConstraints(int numRequiredConstraints, int requestIndex, int month)
    {
        int finalJumpRequestIndex = requestIndex;
        int finalJumpMonth = month;
        int finalJumpConstraintWeight = -1;

        for (int i = 0; i < numRequiredConstraints; i++)
        {
            IConstraintRule constraint = requiredConstraints[i];

            constraint.GetJumpPosition(
                rotationSchedule,
                requests,
                requestIndex,
                month,
                out int newJumpRequestIndex,
                out int newJumpMonth
            );

            if (constraint.Weight > finalJumpConstraintWeight)
            {
                finalJumpMonth = newJumpMonth;
                finalJumpRequestIndex = newJumpRequestIndex;
                finalJumpConstraintWeight = constraint.Weight;
            }
        }

        JumpBackToPosition(requestIndex, month, finalJumpRequestIndex, finalJumpMonth);
    }

    public bool CheckScheduleCompletion()
    {
        foreach (KeyValuePair<Resident, PGY4RotationTypeEnum?[]> entry in rotationSchedule)
        {
            foreach (PGY4RotationTypeEnum? rotationInMonth in entry.Value)
            {
                if (rotationInMonth == null)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool IsValidRotationTypeAssignment(
        int requestIndex,
        int month,
        PGY4RotationTypeEnum rotationType
    )
    {
        Resident resident = requests[requestIndex].Requester;
        foreach (IConstraintRule constraintRule in constraints)
        {
            if (constraintRule.IsValidAssignment(rotationSchedule, resident, month, rotationType))
            {
                continue;
            }
            else
            {
                // Breaks constraint rule
                return false;
            }
        }

        return true;
    }

    private bool IsPreviousRotationNull(int requestIndex, int month)
    {
        if (requestIndex == 0 && month == 0)
        {
            return false;
        }

        PGY4RotationTypeEnum? previousRotationType = null;
        if (requestIndex == 0)
        {
            if (month > 0)
            {
                Resident previousResident = requests[^1].Requester;
                previousRotationType = rotationSchedule[previousResident][month - 1];
            }
        }
        else
        {
            Resident previousResident = requests[requestIndex - 1].Requester;
            previousRotationType = rotationSchedule[previousResident][month];
        }

        if (previousRotationType == null)
        {
            return true;
        }

        return false;
    }

    private void JumpBackToPosition(
        int requestIndex,
        int month,
        int jumpBackRequestIndex,
        int jumpBackMonth
    )
    {
        if (jumpBackMonth != month)
        {
            // Jumping 1 month or more

            for (int i = jumpBackMonth; i <= month; i++)
            {
                if (i == jumpBackMonth)
                {
                    // First erase month, erase from request index to bottom
                    for (int j = jumpBackRequestIndex; j < requests.Length; j++)
                    {
                        Resident jumpResident = requests[j].Requester;
                        rotationSchedule[jumpResident][jumpBackMonth] = null;
                    }
                }
                else if (i == month)
                {
                    // Final erase month, erase from top to request index
                    for (int j = 0; j <= requestIndex; j++)
                    {
                        Resident jumpResident = requests[j].Requester;
                        rotationSchedule[jumpResident][month] = null;
                    }
                }
                else
                {
                    // In between month, erase everything
                    for (int j = 0; j < requests.Length; j++)
                    {
                        Resident jumpResident = requests[j].Requester;
                        rotationSchedule[jumpResident][i] = null;
                    }
                }
            }
        }
        else
        {
            // Jumping back to the same month

            for (int i = jumpBackRequestIndex; i <= requestIndex; i++)
            {
                Resident jumpResident = requests[i].Requester;
                rotationSchedule[jumpResident][month] = null;
            }
        }
    }

    private PGY4RotationTypeEnum[] GetPreferredRotationTypeEnum(
        AlgorithmRotationPrefRequest request,
        int month,
        IConstraintRule[] requiredConstraints,
        out int numRequiredConstraints
    )
    {
        //  Ensure all constraints first before choosing resident's preferred rotations
        HashSet<PGY4RotationTypeEnum> allowedRotations = [.. Enum.GetValues<PGY4RotationTypeEnum>()];
        bool requiredConstraintRotationsFound = false;
        numRequiredConstraints = 0;

        foreach (IConstraintRule constraintRule in constraints)
        {
            HashSet<PGY4RotationTypeEnum> requiredRotations =
                constraintRule.GetRequiredRotationByConstraint(
                    rotationSchedule,
                    request.Requester,
                    month
                );

            if (requiredRotations.Count != 0)
            {
                requiredConstraintRotationsFound = true;
                allowedRotations.IntersectWith(requiredRotations);

                requiredConstraints[numRequiredConstraints] = constraintRule;
                numRequiredConstraints++;
            }
        }

        if (requiredConstraintRotationsFound)
        {
            return [.. allowedRotations];
        }

        allowedRotations = [.. Enum.GetValues<PGY4RotationTypeEnum>()];
        //  Remove any rotations that the constraints does not allow

        foreach (IConstraintRule constraintRule in constraints)
        {
            HashSet<PGY4RotationTypeEnum> blockedRotations = constraintRule.GetBlockedRotationByConstraint(
                rotationSchedule,
                request.Requester,
                month
            );
            if (blockedRotations.Count != 0)
            {
                foreach (PGY4RotationTypeEnum blockedRotation in blockedRotations)
                {
                    allowedRotations.Remove(blockedRotation);
                }
            }
        }

        // Rank rotations by resident's preferences
        PGY4RotationTypeEnum[] rankedRotations = new PGY4RotationTypeEnum[allowedRotations.Count];

        // Populate priorities, alternatives, and avoids
        int frontIndex = 0;
        foreach (PGY4RotationTypeEnum priority in request.Priorities)
        {
            if (allowedRotations.Remove(priority))
            {
                rankedRotations[frontIndex++] = priority;
            }
        }

        foreach (PGY4RotationTypeEnum alternative in request.Alternatives)
        {
            if (allowedRotations.Remove(alternative))
            {
                rankedRotations[frontIndex++] = alternative;
            }
        }

        int backIndex = rankedRotations.Length - 1;
        foreach (PGY4RotationTypeEnum avoid in request.Avoids)
        {
            if (allowedRotations.Remove(avoid))
            {
                rankedRotations[backIndex--] = avoid;
            }
        }

        foreach (PGY4RotationTypeEnum leftOver in allowedRotations)
        {
            rankedRotations[frontIndex++] = leftOver;
        }

        // Choose from ranked rotations by using scoring them

        Dictionary<PGY4RotationTypeEnum, double> preferenceScores = [];

        int numInpatientsRequired = 2;
        int numConsultsRequired = 2;
        int monthRemaining = totalMonths - month - 1;

        for (int i = 0; i < rankedRotations.Length; i++)
        {
            // Initial score = N - 1
            preferenceScores.Add(rankedRotations[i], rankedRotations.Length - i);
        }

        // Apply penalty if rotation was already chosen once or more in the previous month
        foreach (PGY4RotationTypeEnum? rotation in rotationSchedule[request.Requester])
        {
            if (rotation == null)
            {
                break;
            }

            if (rotation is PGY4RotationTypeEnum r && preferenceScores.ContainsKey(r))
            {
                preferenceScores[r] -= PER_MONTH_DIVERSITY_PENALTY;
            }

            if (rotation == PGY4RotationTypeEnum.InpatientPsy)
            {
                numInpatientsRequired--;
                numInpatientsRequired = numInpatientsRequired < 0 ? 0 : numInpatientsRequired;
            }
            else if (rotation == PGY4RotationTypeEnum.PsyConsults)
            {
                numConsultsRequired--;
                numConsultsRequired = numConsultsRequired < 0 ? 0 : numConsultsRequired;
            }

            monthRemaining--;
        }

        // Apply penalty if rotation was already chosen once or more by other residents
        for (int i = 0; i < requests.Length; i++)
        {
            Resident resident = requests[i].Requester;
            PGY4RotationTypeEnum? rotation = rotationSchedule[resident][month];

            if (rotation == null)
            {
                continue;
            }

            if (rotation is PGY4RotationTypeEnum r && preferenceScores.ContainsKey(r))
            {
                preferenceScores[r] -= SINGLE_MONTH_DIVERSITY_PENALTY;
            }
        }

        // Boost required rotations: Inpatient, consults

        if (monthRemaining > 0)
        {
            if (preferenceScores.ContainsKey(PGY4RotationTypeEnum.InpatientPsy))
            {
                preferenceScores[PGY4RotationTypeEnum.InpatientPsy] +=
                    numInpatientsRequired * REQUIRED_ROTATION_URGENCY_MULTIPLIER;
            }
            if (preferenceScores.ContainsKey(PGY4RotationTypeEnum.PsyConsults))
            {
                preferenceScores[PGY4RotationTypeEnum.PsyConsults] +=
                    numConsultsRequired * REQUIRED_ROTATION_URGENCY_MULTIPLIER;
            }
        }

        // Apply penalty if same rotation is picked in the previous month
        if (month != 0)
        {
            PGY4RotationTypeEnum? previousMonthRotation = rotationSchedule[request.Requester][month - 1];

            if (previousMonthRotation is PGY4RotationTypeEnum r && preferenceScores.ContainsKey(r))
            {
                preferenceScores[r] -= CLUSTER_PENALTY;
            }
        }

        // Do a little bit of randomization
        PGY4RotationTypeEnum[] rankedRotationTypeEnum =
        [
            .. preferenceScores
                .OrderByDescending(keyValuePair => keyValuePair.Value)
                .Select(keyValuePair => keyValuePair.Key),
        ];

        double randomDouble = seededRandom.NextDouble();

        if (rankedRotationTypeEnum.Length >= 2 && randomDouble < CHANCE_OF_PRIO_SWAPPING)
        {
            // Swap first and second rotation types
            (rankedRotations[1], rankedRotations[0]) = (rankedRotations[0], rankedRotations[1]);
        }

        randomDouble = seededRandom.NextDouble();

        if (rankedRotationTypeEnum.Length >= 3 && randomDouble < CHANCE_OF_PRIO_SWAPPING)
        {
            // Swap second and third rotation types
            (rankedRotations[2], rankedRotations[1]) = (rankedRotations[1], rankedRotations[2]);
        }

        return rankedRotationTypeEnum;
    }

    public void PrintSchedule()
    {
        // Header
        Console.WriteLine(string.Concat(Enumerable.Repeat("*", 144)));
        Console.WriteLine("Generated Schedule: ");
        Console.WriteLine(string.Concat(Enumerable.Repeat("*", 144)));

        Console.WriteLine(string.Concat(Enumerable.Repeat("-", 144)));

        Console.Write($"|{"Name",-10}");

        for (int i = 1; i <= totalMonths; i++)
        {
            Console.Write($"|    {i,-6}");
        }

        Console.WriteLine("|");
        Console.WriteLine(string.Concat(Enumerable.Repeat("-", 144)));

        // Content
        foreach (KeyValuePair<Resident, PGY4RotationTypeEnum?[]> entry in rotationSchedule)
        {
            Console.Write($"|{entry.Key.FirstName,-10}");
            foreach (PGY4RotationTypeEnum? rotationInMonth in entry.Value)
            {
                if (rotationInMonth?.ToString().Length > 10)
                {
                    Console.Write($"|{rotationInMonth?.ToString()[..10] ?? "",-10}");
                }
                else
                {
                    Console.Write($"|{rotationInMonth?.ToString() ?? "",-10}");
                }
            }

            Console.WriteLine("|");
        }

        Console.WriteLine(string.Concat(Enumerable.Repeat("-", 144)));
        Console.WriteLine($"Schedule Fitness: {scheduleFitness:0.0}");
    }

    public void PrintScheduleOnlyInpatientConsult()
    {
        // Header
        Console.WriteLine(string.Concat(Enumerable.Repeat("*", 144)));
        Console.WriteLine("Generated Schedule: ");
        Console.WriteLine(string.Concat(Enumerable.Repeat("*", 144)));

        Console.WriteLine(string.Concat(Enumerable.Repeat("-", 144)));

        Console.Write($"|{"Name",-10}");

        for (int i = 1; i <= totalMonths; i++)
        {
            Console.Write($"|    {i,-6}");
        }

        Console.WriteLine("|");
        Console.WriteLine(string.Concat(Enumerable.Repeat("-", 144)));

        // Content
        foreach (KeyValuePair<Resident, PGY4RotationTypeEnum?[]> entry in rotationSchedule)
        {
            Console.Write($"|{entry.Key.FirstName,-10}");
            foreach (PGY4RotationTypeEnum? rotationInMonth in entry.Value)
            {
                if (
                    rotationInMonth == PGY4RotationTypeEnum.InpatientPsy
                    || rotationInMonth == PGY4RotationTypeEnum.PsyConsults
                )
                {
                    Console.Write($"|{rotationInMonth?.ToString() ?? "",-10}");
                }
                else
                {
                    Console.Write($"|{"",-10}");
                }
            }

            Console.WriteLine("|");
        }

        Console.WriteLine(string.Concat(Enumerable.Repeat("-", 144)));
        Console.WriteLine($"Schedule Fitness: {scheduleFitness:0.0}");
    }

    private void EvaluatePerformance()
    {
        // Increase score for resident priorities picked
        foreach (KeyValuePair<Resident, PGY4RotationTypeEnum?[]> entry in rotationSchedule)
        {
            AlgorithmRotationPrefRequest residentRequest = requests.First(
                (request) => request.Requester == entry.Key
            );

            HashSet<PGY4RotationTypeEnum> pastRotationTypeEnum = [];

            for (int i = 0; i < entry.Value.Length; i++)
            {
                PGY4RotationTypeEnum? currentMonthRotation = entry.Value[i];

                if (currentMonthRotation == null)
                {
                    continue;
                }

                // Increase score for resident priorities picked
                int priorityIndex = Array.FindIndex(
                    residentRequest.Priorities,
                    priority => priority == currentMonthRotation
                );
                if (priorityIndex != -1)
                {
                    double scoreGained = PRIORITY_GAINED_POINT - priorityIndex;
                    if (scoreGained > 0)
                    {
                        priorityGainedScore += scoreGained;
                    }
                }

                // Increase score for higher per-month diversity
                if (pastRotationTypeEnum.Add((PGY4RotationTypeEnum)currentMonthRotation))
                {
                    perMonthDiversityGainedScore += PER_MONTH_DIVERSITY_GAINED_POINT;
                }

                // Increase score for non clusters
                if (i > 0)
                {
                    PGY4RotationTypeEnum? previousMonthRotation = entry.Value[i - 1];
                    if (previousMonthRotation != currentMonthRotation)
                    {
                        nonClusterGainedScore += NON_CLUSTER_GAINED_POINT;
                    }
                }
            }
        }

        for (int month = 0; month < 12; month++)
        {
            HashSet<PGY4RotationTypeEnum> pastRotationTypeEnum = [];
            int inpatientCount = 0;
            int consultCount = 0;

            foreach (Resident resident in rotationSchedule.Keys)
            {
                // Increase score for higher single-month diversity

                PGY4RotationTypeEnum? currentRotation = rotationSchedule[resident][month];

                if (currentRotation == null)
                {
                    continue;
                }

                if (pastRotationTypeEnum.Add((PGY4RotationTypeEnum)currentRotation))
                {
                    singleMonthGainedScore += SINGLE_MONTH_DIVERSITY_GAINED_POINT;
                }

                if (currentRotation == PGY4RotationTypeEnum.InpatientPsy)
                {
                    inpatientCount++;
                    singleMonthInpatientConsultDiversitScore +=
                        SINGLE_INPATIENT_CONSULT_DIVERSITY_GAINED_POINT / (inpatientCount * 10);
                }
                else if (currentRotation == PGY4RotationTypeEnum.PsyConsults)
                {
                    consultCount++;
                    singleMonthInpatientConsultDiversitScore +=
                        SINGLE_INPATIENT_CONSULT_DIVERSITY_GAINED_POINT / (consultCount * 10);
                }
            }
        }
    }
}