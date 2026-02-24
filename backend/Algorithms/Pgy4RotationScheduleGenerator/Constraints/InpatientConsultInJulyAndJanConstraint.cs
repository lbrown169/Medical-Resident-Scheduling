
using MedicalDemo.Enums;
using MedicalDemo.Models.DTO.Pgy4Scheduling;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Algorithms.Pgy4RotationScheduleGenerator.Constraints;

public class InpatientConsultInJulyAndJanConstraint : IConstraint
{
    public int Weight => 2;

    public bool IsValidAssignment(
        Dictionary<Resident, Pgy4RotationTypeEnum?[]> schedule,
        Resident resident,
        int month,
        Pgy4RotationTypeEnum rotationType,
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
            Pgy4RotationTypeEnum? rotation = schedule[res][month];
            if (rotation != null)
            {
                if (rotation == Pgy4RotationTypeEnum.InpatientPsy)
                {
                    inpatientCount++;
                }
                else if (rotation == Pgy4RotationTypeEnum.PsyConsults)
                {
                    consultsCount++;
                }
            }
            else
            {
                unassignedCount++;
            }
        }

        if (rotationType == Pgy4RotationTypeEnum.InpatientPsy)
        {
            inpatientCount++;
        }
        else if (rotationType == Pgy4RotationTypeEnum.PsyConsults)
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

    public HashSet<Pgy4RotationTypeEnum> GetRequiredRotationByConstraint(
        Dictionary<Resident, Pgy4RotationTypeEnum?[]> schedule,
        Resident resident,
        int month,
        int totalMonths = 12
    )
    {
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
            Pgy4RotationTypeEnum? rotation = schedule[res][month];
            if (rotation != null)
            {
                if (rotation == Pgy4RotationTypeEnum.InpatientPsy)
                {
                    inpatientCount++;
                }
                else if (rotation == Pgy4RotationTypeEnum.PsyConsults)
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

        HashSet<Pgy4RotationTypeEnum> requiredRotations = [];
        if (inpatientCount == 0)
        {
            requiredRotations.Add(Pgy4RotationTypeEnum.InpatientPsy);
        }
        if (consultsCount == 0)
        {
            requiredRotations.Add(Pgy4RotationTypeEnum.PsyConsults);
        }
        return requiredRotations;
    }

    public HashSet<Pgy4RotationTypeEnum> GetBlockedRotationByConstraint(
        Dictionary<Resident, Pgy4RotationTypeEnum?[]> schedule,
        Resident resident,
        int month,
        int totalMonths = 12
    )
    {
        return [];
    }

    public void GetJumpPosition(
        Dictionary<Resident, Pgy4RotationTypeEnum?[]> schedule,
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
            Pgy4RotationTypeEnum? rotation = schedule[resident][month];

            if (rotation == null)
            {
                return;
            }

            if (rotation != Pgy4RotationTypeEnum.InpatientPsy && rotation != Pgy4RotationTypeEnum.PsyConsults)
            {
                newRequestIndex = i;
            }
        }
    }
}