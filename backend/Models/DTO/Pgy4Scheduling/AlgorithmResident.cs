using MedicalDemo.Enums;

namespace MedicalDemo.Models.DTO.Pgy4Scheduling;

public class AlgorithmResident
{
    public string ResidentId { get; set; } = null!;

    public ChiefType ChiefType { get; set; }
}