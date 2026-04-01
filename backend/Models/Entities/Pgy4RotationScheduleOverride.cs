using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MedicalDemo.Enums;

namespace MedicalDemo.Models.Entities;

[Table("pgy4_rotation_schedule_override")]
public class Pgy4RotationScheduleOverride
{
    [Key]
    public Guid Pgy4RotationScheduleOverrideId { get; set; }

    public Guid Pgy4RotationScheduleId { get; set; }

    public MonthOfYear RotationMonthOfYearOverride { get; set; }

    public string ResidentOverrideId { get; set; } = null!;

    public Guid RotationTypeOverrideId { get; set; }

    [ForeignKey(nameof(ResidentOverrideId))]
    public Resident Resident { get; set; } = null!;

    [ForeignKey(nameof(Pgy4RotationScheduleId))]
    public Pgy4RotationSchedule Pgy4RotationSchedule { get; set; } = null!;

    [ForeignKey(nameof(RotationTypeOverrideId))]
    public RotationType RotationType { get; set; } = null!;
}