
using MedicalDemo.Enums;
using MedicalDemo.Models.Entities;

namespace RotationScheduleGenerator.Algorithm;

public class Min2ConsultsInpatient : IConstraintRule
{
    public int Weight => 3;

    public bool IsValidAssignment(
        Dictionary<Resident, PGY4RotationTypeEnum?[]> schedule,
        Resident resident,
        int month,
        PGY4RotationTypeEnum rotationType,
        int totalMonths = 12
    )
    {
        const int requiredConsults = 2;
        const int requiredInpatients = 2;

        int consultsCount = 0;
        int inpatientPsychCount = 0;

        for (int i = 0; i < totalMonths; i++)
        {
            PGY4RotationTypeEnum? rotation = schedule[resident][i];

            if (rotation == null)
            {
                break;
            }

            if (rotation == PGY4RotationTypeEnum.PsyConsults)
            {
                consultsCount++;
            }
            else if (rotation == PGY4RotationTypeEnum.InpatientPsy)
            {
                inpatientPsychCount++;
            }
        }

        if (rotationType == PGY4RotationTypeEnum.PsyConsults)
        {
            consultsCount++;
        }
        else if (rotationType == PGY4RotationTypeEnum.InpatientPsy)
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

    public HashSet<PGY4RotationTypeEnum> GetRequiredRotationByConstraint(
        Dictionary<Resident, PGY4RotationTypeEnum?[]> schedule,
        Resident resident,
        int month,
        int totalMonths = 12
    )
    {
        int requiredConsults = 2;
        int requiredInpatients = 2;

        for (int i = 0; i < totalMonths; i++)
        {
            PGY4RotationTypeEnum? rotation = schedule[resident][i];

            if (rotation == null)
            {
                break;
            }

            if (rotation == PGY4RotationTypeEnum.PsyConsults)
            {
                requiredConsults--;
            }
            else if (rotation == PGY4RotationTypeEnum.InpatientPsy)
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
            HashSet<PGY4RotationTypeEnum> requiredRotations = [];
            if (requiredConsults > 0)
            {
                requiredRotations.Add(PGY4RotationTypeEnum.PsyConsults);
            }
            if (requiredInpatients > 0)
            {
                requiredRotations.Add(PGY4RotationTypeEnum.InpatientPsy);
            }
            return requiredRotations;
        }
    }

    public HashSet<PGY4RotationTypeEnum> GetBlockedRotationByConstraint(
        Dictionary<Resident, PGY4RotationTypeEnum?[]> schedule,
        Resident resident,
        int month,
        int totalMonths = 12
    )
    {
        return [];
    }

    public void GetJumpPosition(
        Dictionary<Resident, PGY4RotationTypeEnum?[]> schedule,
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
        Resident resident = request.Requester;

        newRequestIndex = requestIndex;
        newMonth = 0;

        for (int i = 0; i < totalMonths; i++)
        {
            PGY4RotationTypeEnum? rotation = schedule[resident][i];
            if (rotation == null)
            {
                return;
            }

            if (rotation != PGY4RotationTypeEnum.InpatientPsy && rotation != PGY4RotationTypeEnum.PsyConsults)
            {
                newMonth = i;
            }
        }
    }
}
