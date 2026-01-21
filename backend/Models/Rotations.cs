using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalDemo.Models;

[Table("rotations")]
public class Rotations
{
    [Key]
    [Column("rotation_id")]
    public Guid
        RotationId
    { get; set; } // binary(16) typically maps to Guid in EF Core

    [Column("resident_id")]
    [MaxLength(15)]
    public string ResidentId { get; set; }

    [Column("month")]
    [MaxLength(45)]
    public string Month { get; set; } = null!;

    [Column("month_index")]
    [Required]
    public int MonthIndex { get; set; }

    [Column("year")]
    [Required]
    public int Year { get; set; }

    [Column("pgy_year")]
    [Required]
    public int PgyYear { get; set; }

    [Column("rotation_type_id")]
    [ForeignKey(nameof(RotationTypes))]
    public Guid RotationTypeId { get; set; }

    public RotationTypes RotationType { get; set; } = null!;
}