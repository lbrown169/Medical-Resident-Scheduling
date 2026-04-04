using MedicalDemo.Enums;
using MedicalDemo.Extensions;
using MedicalDemo.Models.DTO.Pgy4Scheduling;

namespace MedicalDemo.Algorithms.Pgy4RotationScheduleGenerator.Constraints;

public class HasChiefRotationConstraint : IConstraint
{
    public int Weight => 1;

    public Pgy4ConstraintType ConstraintType => Pgy4ConstraintType.ChiefConstraint;

    private readonly int[] adminChiefMonths = [4, 6, 10];
    private readonly int[] clinicChiefMonths = [2, 6, 9];
    private readonly int[] educationChiefMonths = [0, 7];

    public bool IsValidAssignment(
        Dictionary<AlgorithmResident, Pgy4RotationTypeEnum?[]> schedule,
        AlgorithmResident resident,
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
        Dictionary<AlgorithmResident, Pgy4RotationTypeEnum?[]> schedule,
        AlgorithmResident resident,
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
        Dictionary<AlgorithmResident, Pgy4RotationTypeEnum?[]> schedule,
        AlgorithmResident resident,
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
        Dictionary<AlgorithmResident, Pgy4RotationTypeEnum?[]> schedule,
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

    public Pgy4ConstraintViolation GetRotationScheduleConstraintViolations(
        Dictionary<AlgorithmResident, Pgy4RotationTypeEnum[]> schedule
    )
    {
        List<Pgy4ConstraintError> errors = [];

        const string missingChiefRotationOnMonthErrorTemplate =
            "Resident {0} is missing Chief rotation on {1}";
        const string chiefRotationOnInvalidMonthErrorTemplate =
            "Resident {0} is not allowed to take chief rotation on {1}";
        const string chiefRotationOnNonChiefResidentErrorTemplate =
            "Resident {0} is not a chief resident but is assigned a chief rotation on {1}";

        foreach (KeyValuePair<AlgorithmResident, Pgy4RotationTypeEnum[]> kvp in schedule)
        {
            AlgorithmResident resident = kvp.Key;
            Pgy4RotationTypeEnum[] rotations = kvp.Value;

            for (int monthIndex = 0; monthIndex < rotations.Length; monthIndex++)
            {
                MonthOfYear calendarMonth = MonthOfYearExtensions.FromCalendarIndex(
                    monthIndex,
                    false
                );

                if (resident.ChiefType != ChiefType.None)
                {
                    // Is chief resident

                    bool requiresChief = DoesRequiredChief(resident.ChiefType, monthIndex);

                    // Check missingChiefrotationOnMonth
                    if (requiresChief && rotations[monthIndex] != Pgy4RotationTypeEnum.Chief)
                    {
                        string errorMessage = string.Format(
                            format: missingChiefRotationOnMonthErrorTemplate,
                            $"{resident.FirstName} {resident.LastName}",
                            calendarMonth
                        );

                        errors.Add(
                            new()
                            {
                                Message = errorMessage,
                                MonthIndex = calendarMonth,
                                Resident = resident,
                            }
                        );
                        continue;
                    }

                    // Check chiefRotationOnInvalidMonth
                    if (!requiresChief && rotations[monthIndex] == Pgy4RotationTypeEnum.Chief)
                    {
                        string errorMessage = string.Format(
                            format: chiefRotationOnInvalidMonthErrorTemplate,
                            $"{resident.FirstName} {resident.LastName}",
                            calendarMonth
                        );

                        errors.Add(
                            new()
                            {
                                Message = errorMessage,
                                MonthIndex = calendarMonth,
                                Resident = resident,
                            }
                        );
                        continue;
                    }
                }
                else
                {
                    // Is not chief resident
                    if (rotations[monthIndex] == Pgy4RotationTypeEnum.Chief)
                    {
                        string errorMessage = string.Format(
                            chiefRotationOnNonChiefResidentErrorTemplate,
                            $"{resident.FirstName} {resident.LastName}",
                            calendarMonth
                        );

                        errors.Add(
                            new()
                            {
                                Message = errorMessage,
                                MonthIndex = calendarMonth,
                                Resident = resident,
                            }
                        );

                        continue;
                    }
                }
            }
        }

        return new() { ConstraintViolated = ConstraintType, Errors = errors };
    }
}