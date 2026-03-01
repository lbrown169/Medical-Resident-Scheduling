using MedicalDemo.Enums;
using MedicalDemo.Models.DTO.Pgy4Scheduling;

namespace MedicalDemo.Algorithms.Pgy4RotationScheduleGenerator.Constraints;

public class Min2ConsultsInpatientConstraint : IConstraint
{
    public int Weight => 3;

    public Pgy4ConstraintType ConstraintType => Pgy4ConstraintType.Min2ConsultsInpatients;

    public bool IsValidAssignment(
        Dictionary<AlgorithmResident, Pgy4RotationTypeEnum?[]> schedule,
        AlgorithmResident resident,
        int month,
        Pgy4RotationTypeEnum rotationType,
        int totalMonths = 12
    )
    {
        const int requiredConsults = 2;
        const int requiredInpatients = 2;

        int consultsCount = 0;
        int inpatientPsychCount = 0;

        for (int i = 0; i < totalMonths; i++)
        {
            Pgy4RotationTypeEnum? rotation = schedule[resident][i];

            if (rotation == null)
            {
                break;
            }

            if (rotation == Pgy4RotationTypeEnum.PsyConsults)
            {
                consultsCount++;
            }
            else if (rotation == Pgy4RotationTypeEnum.InpatientPsy)
            {
                inpatientPsychCount++;
            }
        }

        if (rotationType == Pgy4RotationTypeEnum.PsyConsults)
        {
            consultsCount++;
        }
        else if (rotationType == Pgy4RotationTypeEnum.InpatientPsy)
        {
            inpatientPsychCount++;
        }

        if (consultsCount > 2)
        {
            consultsCount = 2;
        }
        if (inpatientPsychCount > 2)
        {
            inpatientPsychCount = 2;
        }

        int remainingMonths = totalMonths - month - 1;
        return consultsCount + inpatientPsychCount + remainingMonths
            >= requiredConsults + requiredInpatients;
    }

    public HashSet<Pgy4RotationTypeEnum> GetRequiredRotationByConstraint(
        Dictionary<AlgorithmResident, Pgy4RotationTypeEnum?[]> schedule,
        AlgorithmResident resident,
        int month,
        int totalMonths = 12
    )
    {
        int requiredConsults = 2;
        int requiredInpatients = 2;

        for (int i = 0; i < totalMonths; i++)
        {
            Pgy4RotationTypeEnum? rotation = schedule[resident][i];

            if (rotation == null)
            {
                break;
            }

            if (rotation == Pgy4RotationTypeEnum.PsyConsults)
            {
                requiredConsults--;
            }
            else if (rotation == Pgy4RotationTypeEnum.InpatientPsy)
            {
                requiredInpatients--;
            }
        }

        int remainingMonths = totalMonths - month;
        if (remainingMonths > requiredConsults + requiredInpatients)
        {
            return [];
        }
        else
        {
            HashSet<Pgy4RotationTypeEnum> requiredRotations = [];
            if (requiredConsults > 0)
            {
                requiredRotations.Add(Pgy4RotationTypeEnum.PsyConsults);
            }
            if (requiredInpatients > 0)
            {
                requiredRotations.Add(Pgy4RotationTypeEnum.InpatientPsy);
            }
            return requiredRotations;
        }
    }

    public HashSet<Pgy4RotationTypeEnum> GetBlockedRotationByConstraint(
        Dictionary<AlgorithmResident, Pgy4RotationTypeEnum?[]> schedule,
        AlgorithmResident resident,
        int month,
        int totalMonths = 12
    )
    {
        return [];
    }

    public void GetJumpPosition(
        Dictionary<AlgorithmResident, Pgy4RotationTypeEnum?[]> schedule,
        AlgorithmRotationPrefRequest[] requests,
        int requestIndex,
        int month,
        out int newRequestIndex,
        out int newMonth,
        int totalMonths = 12
    )
    {
        if (month == 0)
        {
            newRequestIndex = requestIndex;
            newMonth = month;
            return;
        }

        AlgorithmRotationPrefRequest request = requests[requestIndex];
        AlgorithmResident resident = request.Requester;

        newRequestIndex = requestIndex;
        newMonth = 0;

        for (int i = 0; i < totalMonths; i++)
        {
            Pgy4RotationTypeEnum? rotation = schedule[resident][i];
            if (rotation == null)
            {
                return;
            }

            if (
                rotation != Pgy4RotationTypeEnum.InpatientPsy
                && rotation != Pgy4RotationTypeEnum.PsyConsults
            )
            {
                newMonth = i;
            }
        }
    }

    public Pgy4ConstraintViolation GetRotationScheduleConstraintViolations(
        Dictionary<AlgorithmResident, Pgy4RotationTypeEnum[]> schedule
    )
    {
        List<Pgy4ConstraintError> errors = [];

        const string missingRotationOnResidentErrorTemplate =
            "Resident {0} is missing {1} {2} rotation(s)";

        foreach (KeyValuePair<AlgorithmResident, Pgy4RotationTypeEnum[]> kvp in schedule)
        {
            AlgorithmResident resident = kvp.Key;
            Pgy4RotationTypeEnum[] rotations = kvp.Value;

            int numInpatientsEncountered = 0;
            int numConsultsEncountered = 0;

            for (int monthIndex = 0; monthIndex < rotations.Length; monthIndex++)
            {
                if (rotations[monthIndex] == Pgy4RotationTypeEnum.InpatientPsy)
                {
                    numInpatientsEncountered++;
                }
                else if (rotations[monthIndex] == Pgy4RotationTypeEnum.PsyConsults)
                {
                    numConsultsEncountered++;
                }
            }

            if (numInpatientsEncountered < 2)
            {
                string errorMessage = string.Format(
                    missingRotationOnResidentErrorTemplate,
                    $"{resident.FirstName} {resident.LastName}",
                    2 - numInpatientsEncountered,
                    Pgy4RotationTypeEnum.InpatientPsy
                );

                errors.Add(
                    new()
                    {
                        Message = errorMessage,
                        CalendarMonthIndex = null,
                        Resident = resident,
                    }
                );
            }

            if (numConsultsEncountered < 2)
            {
                string errorMessage = string.Format(
                    missingRotationOnResidentErrorTemplate,
                    $"{resident.FirstName} {resident.LastName}",
                    2 - numConsultsEncountered,
                    Pgy4RotationTypeEnum.PsyConsults
                );

                errors.Add(
                    new()
                    {
                        Message = errorMessage,
                        CalendarMonthIndex = null,
                        Resident = resident,
                    }
                );
            }
        }

        return new() { ConstraintViolated = ConstraintType, Errors = errors };
    }
}