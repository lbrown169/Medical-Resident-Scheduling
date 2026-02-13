using MedicalDemo.Enums;
using MedicalDemo.Models.Entities;

namespace RotationScheduleGenerator.Algorithm;

public interface IConstraintRule
{
    public int Weight { get; }

    public bool IsValidAssignment(
        Dictionary<Resident, PGY4RotationTypeEnum?[]> schedule,
        Resident resident,
        int month,
        PGY4RotationTypeEnum rotationType,
        int totalMonths = 12
    );

    // Returns a list of rotation types that the residents can only have
    public HashSet<PGY4RotationTypeEnum> GetRequiredRotationByConstraint(
        Dictionary<Resident, PGY4RotationTypeEnum?[]> schedule,
        Resident resident,
        int month,
        int totalMonths = 12
    );

    public HashSet<PGY4RotationTypeEnum> GetBlockedRotationByConstraint(
        Dictionary<Resident, PGY4RotationTypeEnum?[]> schedule,
        Resident resident,
        int month,
        int totalMonths = 12
    );

    public void GetJumpPosition(
        Dictionary<Resident, PGY4RotationTypeEnum?[]> schedule,
        AlgorithmRotationPrefRequest[] requests,
        int requestIndex,
        int month,
        out int newRequestIndex,
        out int newMonth,
        int totalMonths = 12
    );
}
