namespace MedicalDemo.Models.DTO.Responses;

public class Pgy4RotationScheduleOverrideResponse
{
    public Guid Pgy4RotationScheduleOverrideId { get; set; }

    public Guid Pgy4RotationScheduleId { get; set; }

    public int AcademicMonthIndex { get; set; }

    public ResidentResponse? Resident { get; set; } = null!;

    public RotationTypeResponse OverrideRotation { get; set; } = null!;
}