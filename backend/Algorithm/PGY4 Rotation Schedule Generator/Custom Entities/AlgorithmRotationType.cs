using MedicalDemo.Enums;

namespace RotationScheduleGenerator.Algorithm;

public class AlgorithmRotationType
{
    public Guid RotationTypeId { get; set; }

    public PGY4RotationTypeEnum Type { get; set; }

    public string RotationName { get; set; } = null!;

    public bool IsChiefRotation { get; set; }
}
