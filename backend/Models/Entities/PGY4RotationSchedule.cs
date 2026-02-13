using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalDemo.Models.Entities;

[Table("pgy4_rotation_schedule")]
public class PGY4RotationSchedule
{
    [Key]
    [Column("pgy4_rotation_schedule_id")]
    public Guid PGY4RotationScheduleId { get; set; }

    [Column("seed")]
    public string Seed { get; set; } = null!;

    [ForeignKey(nameof(PGY4RotationScheduleId))]
    public virtual List<Rotation> Rotations { get; set; } = [];
}