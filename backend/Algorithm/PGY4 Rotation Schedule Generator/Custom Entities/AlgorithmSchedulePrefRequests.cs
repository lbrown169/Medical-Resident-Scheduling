using MedicalDemo.Models.Entities;

namespace RotationScheduleGenerator.Algorithm;

public class AlgorithmRotationPrefRequest
{
    public Guid RotationPrefRequestId { get; set; }

    public Resident Requester { get; set; } = null!;

    public AlgorithmRotationType[] Priorities { get; set; } = null!;

    public AlgorithmRotationType[] Alternatives { get; set; } = null!;

    public AlgorithmRotationType[] Avoids { get; set; } = null!;
}
