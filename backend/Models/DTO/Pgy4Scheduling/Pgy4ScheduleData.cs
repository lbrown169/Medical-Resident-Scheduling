using MedicalDemo.Enums;

namespace MedicalDemo.Models.DTO.Pgy4Scheduling;

public class Pgy4ScheduleData
{
    public Dictionary<string, Pgy4RotationTypeEnum[]> Schedule { get; set; } = null!;
}