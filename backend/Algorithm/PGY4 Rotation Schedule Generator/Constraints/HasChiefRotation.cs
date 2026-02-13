
using MedicalDemo.Enums;
using MedicalDemo.Models.Entities;

namespace RotationScheduleGenerator.Algorithm;

public class HasChiefRotation : IConstraintRule
{
    public int Weight => 1;

    public bool IsValidAssignment(
        Dictionary<Resident, PGY4RotationTypeEnum?[]> schedule,
        Resident resident,
        int month,
        PGY4RotationTypeEnum rotationType,
        int totalMonths = 12
    )
    {
        bool isChiefRotation = rotationType == PGY4RotationTypeEnum.Chief;

        // Resident is not a chief
        if (resident.ChiefType == ChiefType.None)
        {
            if (isChiefRotation)
            {
                return false;
            }
            return true;
        }

        bool requiresChief = resident.ChiefType switch
        {
            ChiefType.Admin => month is 4 or 6 or 10,
            ChiefType.Clinic => month is 2 or 6 or 9,
            ChiefType.Education => month is 0 or 7,
            _ => false,
        };

        if (!requiresChief)
        {
            if (isChiefRotation)
            {
                return false;
            }
            return true;
        }

        return isChiefRotation;
    }

    public HashSet<PGY4RotationTypeEnum> GetRequiredRotationByConstraint(
        Dictionary<Resident, PGY4RotationTypeEnum?[]> schedule,
        Resident resident,
        int month,
        int totalMonths = 12
    )
    {
        // Only allow chief rotation if resident is chief and it's chief rotation month
        if (resident.ChiefType != ChiefType.None)
        {
            bool requiresChief = resident.ChiefType switch
            {
                ChiefType.Admin => month is 4 or 6 or 10,
                ChiefType.Clinic => month is 2 or 6 or 9,
                ChiefType.Education => month is 0 or 7,
                _ => false,
            };

            if (requiresChief)
            {
                return [PGY4RotationTypeEnum.Chief];
            }
        }

        return [];
    }

    public HashSet<PGY4RotationTypeEnum> GetBlockedRotationByConstraint(
        Dictionary<Resident, PGY4RotationTypeEnum?[]> schedule,
        Resident resident,
        int month,
        int totalMonths = 12
    )
    {
        if (resident.ChiefType != ChiefType.None)
        {
            return [PGY4RotationTypeEnum.Chief];
        }
        else
        {
            // Is chief

            bool requiresChief = resident.ChiefType switch
            {
                ChiefType.Admin => month is 4 or 6 or 10,
                ChiefType.Clinic => month is 2 or 6 or 9,
                ChiefType.Education => month is 0 or 7,
                _ => false,
            };

            if (!requiresChief)
            {
                return [PGY4RotationTypeEnum.Chief];
            }
        }
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
        if (month > 0)
        {
            newRequestIndex = requestIndex;
            newMonth = month - 1;
        }
        else
        {
            newRequestIndex = 0;
            newMonth = month;
        }
    }
}
