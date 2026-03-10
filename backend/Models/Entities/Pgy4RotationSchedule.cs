using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalDemo.Models.Entities;

[Table("pgy4_rotation_schedule")]
public class Pgy4RotationSchedule
{
    [Key]
    public Guid Pgy4RotationScheduleId { get; set; }

    public int Seed { get; set; }

    public int Year { get; set; }

    public bool IsPublished { get; set; }

    public virtual List<Rotation> Rotations { get; set; } = [];
}