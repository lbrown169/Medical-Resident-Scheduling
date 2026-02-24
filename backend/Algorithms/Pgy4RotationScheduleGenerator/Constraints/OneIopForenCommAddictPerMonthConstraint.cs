
using MedicalDemo.Enums;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Algorithms.Pgy4RotationScheduleGenerator.Constraints;

public class OneIopForenCommAddictPerMonthConstraint : IConstraint
{
    public int Weight => 0;

    public bool IsValidAssignment(
        Dictionary<Resident, Pgy4RotationTypeEnum?[]> schedule,
        Resident resident,
        int month,
        Pgy4RotationTypeEnum rotationType,
        int totalMonths = 12
    )
    {
        if (
            rotationType
            is not (
                Pgy4RotationTypeEnum.IOP
                or Pgy4RotationTypeEnum.Forensic
                or Pgy4RotationTypeEnum.CommunityPsy
                or Pgy4RotationTypeEnum.Addiction
            )
        )
        {
            return true;
        }

        foreach (Resident res in schedule.Keys)
        {
            Pgy4RotationTypeEnum? rotation = schedule[res][month];
            if (rotation != null && rotation == rotationType)
            {
                return false;
            }
        }

        return true;
    }

    public HashSet<Pgy4RotationTypeEnum> GetRequiredRotationByConstraint(
        Dictionary<Resident, Pgy4RotationTypeEnum?[]> schedule,
        Resident resident,
        int month,
        int totalMonths = 12
    )
    {
        return [];
    }

    public HashSet<Pgy4RotationTypeEnum> GetBlockedRotationByConstraint(
        Dictionary<Resident, Pgy4RotationTypeEnum?[]> schedule,
        Resident resident,
        int month,
        int totalMonths = 12
    )
    {
        HashSet<Pgy4RotationTypeEnum> blockedRotations = [];

        foreach (Resident res in schedule.Keys)
        {
            Pgy4RotationTypeEnum? rotation = schedule[res][month];
            if (rotation != null)
            {
                if (rotation == Pgy4RotationTypeEnum.IOP)
                {
                    blockedRotations.Add(Pgy4RotationTypeEnum.IOP);
                }
                else if (rotation == Pgy4RotationTypeEnum.Forensic)
                {
                    blockedRotations.Add(Pgy4RotationTypeEnum.Forensic);
                }
                else if (rotation == Pgy4RotationTypeEnum.CommunityPsy)
                {
                    blockedRotations.Add(Pgy4RotationTypeEnum.CommunityPsy);
                }
                if (rotation == Pgy4RotationTypeEnum.Addiction)
                {
                    blockedRotations.Add(Pgy4RotationTypeEnum.Addiction);
                }
            }
        }

        return blockedRotations;
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
        // Go to latest IOP, Forensic, Community, Addiction rotation in the same month

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

            if (
                rotation == Pgy4RotationTypeEnum.IOP
                && rotation == Pgy4RotationTypeEnum.Forensic
                && rotation == Pgy4RotationTypeEnum.CommunityPsy
                && rotation == Pgy4RotationTypeEnum.Addiction
            )
            {
                newRequestIndex = i;
            }
        }
    }
}