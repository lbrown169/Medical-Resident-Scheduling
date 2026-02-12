using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MedicalDemo.Enums;

namespace MedicalDemo.Models.Entities;


[Table("rotation_type")]
public class RotationType
{
    [Key]
    [Column("rotation_type_id")]
    public Guid RotationTypeId { get; set; }

    [Column("rotation_name")]
    public string RotationName { get; set; } = null!;

    [Column("does_long_call")]
    public bool DoesLongCall { get; set; }

    [Column("does_short_call")]
    public bool DoesShortCall { get; set; }

    [Column("does_training_long_call")]
    public bool DoesTrainingLongCall { get; set; }

    [Column("does_training_short_call")]
    public bool DoesTrainingShortCall { get; set; }

    [Column("is_chief_rotation")]
    public bool IsChiefRotation { get; set; }

    [Column("pgy_year_flags")]
    public PgyYearFlags PgyYearFlags { get; set; }
}