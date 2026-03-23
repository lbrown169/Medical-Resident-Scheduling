using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MedicalDemo.Enums;

namespace MedicalDemo.Models.Entities;

[Table("rotation_type")]
public class RotationType
{
    [Key]
    public Guid RotationTypeId { get; set; }

    public required string RotationName { get; set; }

    public bool DoesLongCall { get; set; }

    public bool DoesShortCall { get; set; }

    public bool DoesTrainingLongCall { get; set; }

    public bool DoesTrainingShortCall { get; set; }

    public bool IsChiefRotation { get; set; }

    public PgyYearFlags PgyYearFlags { get; set; }
}