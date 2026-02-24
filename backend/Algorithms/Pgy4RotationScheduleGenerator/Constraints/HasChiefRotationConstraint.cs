
using MedicalDemo.Enums;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Algorithms.Pgy4RotationScheduleGenerator.Constraints;

public class HasChiefRotationConstraint : IConstraint
{
    public int Weight => 1;

    private readonly int[] adminChiefMonths = [4, 6, 10];
    private readonly int[] clinicChiefMonths = [2, 6, 9];
    private readonly int[] educationChiefMonths = [0, 7];

    public bool IsValidAssignment(
        Dictionary<Resident, Pgy4RotationTypeEnum?[]> schedule,
        Resident resident,
        int month,
        Pgy4RotationTypeEnum rotationType,
        int totalMonths = 12
    )
    {
        bool isChiefRotation = rotationType == Pgy4RotationTypeEnum.Chief;

        // Resident is not a chief
        if (resident.ChiefType == ChiefType.None)
        {
            if (isChiefRotation)
            {
                return false;
            }
            return true;
        }

        bool requiresChief = DoesRequiredChief(resident.ChiefType, month);

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

    public HashSet<Pgy4RotationTypeEnum> GetRequiredRotationByConstraint(
        Dictionary<Resident, Pgy4RotationTypeEnum?[]> schedule,
        Resident resident,
        int month,
        int totalMonths = 12
    )
    {
        // Only allow chief rotation if resident is chief and it's chief rotation month
        if (resident.ChiefType != ChiefType.None)
        {
            bool requiresChief = DoesRequiredChief(resident.ChiefType, month);

            if (requiresChief)
            {
                return [Pgy4RotationTypeEnum.Chief];
            }
        }

        return [];
    }

    public HashSet<Pgy4RotationTypeEnum> GetBlockedRotationByConstraint(
        Dictionary<Resident, Pgy4RotationTypeEnum?[]> schedule,
        Resident resident,
        int month,
        int totalMonths = 12
    )
    {
        if (resident.ChiefType != ChiefType.None)
        {
            return [Pgy4RotationTypeEnum.Chief];
        }
        else
        {
            // Is chief
            bool requiresChief = DoesRequiredChief(resident.ChiefType, month);

            if (!requiresChief)
            {
                return [Pgy4RotationTypeEnum.Chief];
            }
        }
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

    private bool DoesRequiredChief(ChiefType chiefType, int month)
    {
        return chiefType switch
        {
            ChiefType.Admin => adminChiefMonths.Contains(month),
            ChiefType.Clinic => clinicChiefMonths.Contains(month),
            ChiefType.Education => educationChiefMonths.Contains(month),
            _ => false,
        };
    }
}