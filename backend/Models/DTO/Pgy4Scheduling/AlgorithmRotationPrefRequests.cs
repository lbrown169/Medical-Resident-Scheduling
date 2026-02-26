using MedicalDemo.Enums;

namespace MedicalDemo.Models.DTO.Pgy4Scheduling;

public class AlgorithmRotationPrefRequest
{
    public Guid RotationPrefRequestId { get; set; }

    public AlgorithmResident Requester { get; set; } = null!;

    public Pgy4RotationTypeEnum[] Priorities { get; set; } = null!;

    public Pgy4RotationTypeEnum[] Alternatives { get; set; } = null!;

    public Pgy4RotationTypeEnum[] Avoids { get; set; } = null!;
}