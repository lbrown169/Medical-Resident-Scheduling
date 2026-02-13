
using MedicalDemo.Enums;
using MedicalDemo.Models.Entities;

namespace RotationScheduleGenerator.Algorithm;

public class InpatientConsultInJulyAndJan : IConstraintRule
{
    public int Weight => 2;

    public bool IsValidAssignment(
        Dictionary<Resident, PGY4RotationTypeEnum?[]> schedule,
        Resident resident,
        int month,
        PGY4RotationTypeEnum rotationType,
        int totalMonths = 12
    )
    {
        const int januaryMonth = 0;
        const int julyMonth = 6;

        if (month != januaryMonth && month != julyMonth)
        {
            return true;
        }

        int inpatientCount = 0;
        int consultsCount = 0;
        int unassignedCount = 0;

        foreach (Resident res in schedule.Keys)
        {
            PGY4RotationTypeEnum? rotation = schedule[res][month];
            if (rotation != null)
            {
                if (rotation == PGY4RotationTypeEnum.InpatientPsy)
                {
                    inpatientCount++;
                }
                else if (rotation == PGY4RotationTypeEnum.PsyConsults)
                {
                    consultsCount++;
                }
            }
            else
            {
                unassignedCount++;
            }
        }

        if (rotationType == PGY4RotationTypeEnum.InpatientPsy)
        {
            inpatientCount++;
        }
        else if (rotationType == PGY4RotationTypeEnum.PsyConsults)
        {
            consultsCount++;
        }

        int missingCount = 0;

        if (inpatientCount == 0)
        {
            missingCount++;
        }

        if (consultsCount == 0)
        {
            missingCount++;
        }

        return unassignedCount >= missingCount;
    }

    public HashSet<PGY4RotationTypeEnum> GetRequiredRotationByConstraint(
        Dictionary<Resident, PGY4RotationTypeEnum?[]> schedule,
        Resident resident,
        int month,
        int totalMonths = 12
    )
    {
        // if (IsValidAssignment(schedule, resident, month, RotationTypes.Community))
        // {
        //     return [];
        // }

        // HashSet<RotationTypes> requiredRotations = [];

        // // Missing inpatient and/or consults
        // if (!IsValidAssignment(schedule, resident, month, RotationTypes.Inpatient))
        // {
        //     requiredRotations.Add(RotationTypes.Consults);
        // }

        // if (!IsValidAssignment(schedule, resident, month, RotationTypes.Consults))
        // {
        //     requiredRotations.Add(RotationTypes.Inpatient);
        // }

        // return requiredRotations;

        const int januaryMonth = 0;
        const int julyMonth = 6;

        if (month != januaryMonth && month != julyMonth)
        {
            return [];
        }

        int requiredInpatientCount = 1;
        int requiredConsultCount = 1;

        int inpatientCount = 0;
        int consultsCount = 0;
        int unassignedCount = 0;

        foreach (Resident res in schedule.Keys)
        {
            PGY4RotationTypeEnum? rotation = schedule[res][month];
            if (rotation != null)
            {
                if (rotation == PGY4RotationTypeEnum.InpatientPsy)
                {
                    inpatientCount++;
                }
                else if (rotation == PGY4RotationTypeEnum.PsyConsults)
                {
                    consultsCount++;
                }
            }
            else
            {
                unassignedCount++;
            }
        }

        if (unassignedCount > requiredInpatientCount + requiredConsultCount)
        {
            return [];
        }

        HashSet<PGY4RotationTypeEnum> requiredRotations = [];
        if (inpatientCount == 0)
        {
            requiredRotations.Add(PGY4RotationTypeEnum.InpatientPsy);
        }
        if (consultsCount == 0)
        {
            requiredRotations.Add(PGY4RotationTypeEnum.PsyConsults);
        }
        return requiredRotations;
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
        // Go to latest non inpatient and non consult rotation in same month

        newRequestIndex = requestIndex;
        newMonth = month;

        for (int i = 0; i < requestIndex; i++)
        {
            Resident resident = requests[i].Requester;
            PGY4RotationTypeEnum? rotation = schedule[resident][month];

            if (rotation == null)
            {
                return;
            }

            if (rotation != PGY4RotationTypeEnum.InpatientPsy && rotation != PGY4RotationTypeEnum.PsyConsults)
            {
                newRequestIndex = i;
            }
        }
    }
}
