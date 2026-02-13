
using MedicalDemo.Enums;
using MedicalDemo.Models.Entities;

namespace RotationScheduleGenerator.Algorithm;

public class OneIopForensicCommunityAddictionPerMonth : IConstraintRule
{
    public int Weight => 0;

    public bool IsValidAssignment(
        Dictionary<Resident, PGY4RotationTypeEnum?[]> schedule,
        Resident resident,
        int month,
        PGY4RotationTypeEnum rotationType,
        int totalMonths = 12
    )
    {
        if (
            rotationType
            is not (
                PGY4RotationTypeEnum.IOP
                or PGY4RotationTypeEnum.Forensic
                or PGY4RotationTypeEnum.CommunityPsy
                or PGY4RotationTypeEnum.Addiction
            )
        )
        {
            return true;
        }

        foreach (Resident res in schedule.Keys)
        {
            PGY4RotationTypeEnum? rotation = schedule[res][month];
            if (rotation != null && rotation == rotationType)
            {
                return false;
            }
        }

        return true;
    }

    public HashSet<PGY4RotationTypeEnum> GetRequiredRotationByConstraint(
        Dictionary<Resident, PGY4RotationTypeEnum?[]> schedule,
        Resident resident,
        int month,
        int totalMonths = 12
    )
    {
        return [];
    }

    public HashSet<PGY4RotationTypeEnum> GetBlockedRotationByConstraint(
        Dictionary<Resident, PGY4RotationTypeEnum?[]> schedule,
        Resident resident,
        int month,
        int totalMonths = 12
    )
    {
        HashSet<PGY4RotationTypeEnum> blockedRotations = [];

        foreach (Resident res in schedule.Keys)
        {
            PGY4RotationTypeEnum? rotation = schedule[res][month];
            if (rotation != null)
            {
                if (rotation == PGY4RotationTypeEnum.IOP)
                {
                    blockedRotations.Add(PGY4RotationTypeEnum.IOP);
                }
                else if (rotation == PGY4RotationTypeEnum.Forensic)
                {
                    blockedRotations.Add(PGY4RotationTypeEnum.Forensic);
                }
                else if (rotation == PGY4RotationTypeEnum.CommunityPsy)
                {
                    blockedRotations.Add(PGY4RotationTypeEnum.CommunityPsy);
                }
                if (rotation == PGY4RotationTypeEnum.Addiction)
                {
                    blockedRotations.Add(PGY4RotationTypeEnum.Addiction);
                }
            }
        }

        return blockedRotations;
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
        // Go to latest IOP, Forensic, Community, Addiction rotation in the same month

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

            if (
                rotation == PGY4RotationTypeEnum.IOP
                && rotation == PGY4RotationTypeEnum.Forensic
                && rotation == PGY4RotationTypeEnum.CommunityPsy
                && rotation == PGY4RotationTypeEnum.Addiction
            )
            {
                newRequestIndex = i;
            }
        }
    }
}
