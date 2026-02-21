using MedicalDemo.Enums;
using MedicalDemo.Models.Entities;

namespace RotationScheduleGenerator.Algorithm;

public class AlgorithmRotationPrefRequest
{
    public Guid RotationPrefRequestId { get; set; }

    public Resident Requester { get; set; } = null!;

    public PGY4RotationTypeEnum[] Priorities { get; set; } = null!;

    public PGY4RotationTypeEnum[] Alternatives { get; set; } = null!;

    public PGY4RotationTypeEnum[] Avoids { get; set; } = null!;
}