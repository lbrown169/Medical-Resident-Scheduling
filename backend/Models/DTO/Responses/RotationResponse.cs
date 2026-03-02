using MedicalDemo.Enums;

namespace MedicalDemo.Models.DTO.Responses;

public class RotationResponse
{
    public Guid RotationId { get; set; }

    public Guid? ScheduleId { get; set; }

    public string Month { get; set; } = null!;

    public MonthOfYear AcademicMonthIndex { get; set; }

    public int PgyYear { get; set; }

    public RotationTypeResponse RotationType { get; set; } = null!;

}