using MedicalDemo.Enums;
using MedicalDemo.Models.DTO.Pgy4Scheduling;

namespace MedicalDemo.Algorithms.Pgy4RotationScheduleGenerator.Constraints;

public interface IConstraint
{
    int Weight { get; }

    bool IsValidAssignment(
        Dictionary<AlgorithmResident, Pgy4RotationTypeEnum?[]> schedule,
        AlgorithmResident resident,
        int month,
        Pgy4RotationTypeEnum rotationType,
        int totalMonths = 12
    );

    // Returns a list of rotation types that the residents can only have
    HashSet<Pgy4RotationTypeEnum> GetRequiredRotationByConstraint(
        Dictionary<AlgorithmResident, Pgy4RotationTypeEnum?[]> schedule,
        AlgorithmResident resident,
        int month,
        int totalMonths = 12
    );

    HashSet<Pgy4RotationTypeEnum> GetBlockedRotationByConstraint(
        Dictionary<AlgorithmResident, Pgy4RotationTypeEnum?[]> schedule,
        AlgorithmResident resident,
        int month,
        int totalMonths = 12
    );

    void GetJumpPosition(
        Dictionary<AlgorithmResident, Pgy4RotationTypeEnum?[]> schedule,
        AlgorithmRotationPrefRequest[] requests,
        int requestIndex,
        int month,
        out int newRequestIndex,
        out int newMonth,
        int totalMonths = 12
    );
}