using MedicalDemo.Enums;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Models.DTO.Pgy4Scheduling;

public class Pgy4ScheduleData
{
    public Dictionary<Resident, Pgy4RotationTypeEnum[]> Schedule { get; set; } = null!;
}