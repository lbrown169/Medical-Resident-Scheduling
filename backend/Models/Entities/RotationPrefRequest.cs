using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalDemo.Models.Entities;

[Table("rotation_pref_request")]
public class RotationPrefRequest
{
    [Key]
    public Guid RotationPrefRequestId { get; set; }

    [Column("resident_id")]
    [ForeignKey(nameof(Resident))]
    public string ResidentId { get; set; } = null!;

    [ForeignKey(nameof(ResidentId))]
    public Resident Resident { get; set; } = null!;

    [Column("first_priority_id")]
    [Required]
    public Guid FirstPriorityId { get; set; }

    [Column("second_priority_id")]
    [Required]
    public Guid SecondPriorityId { get; set; }

    [Column("third_priority_id")]
    [Required]
    public Guid ThirdPriorityId { get; set; }

    [Column("fourth_priority_id")]
    [Required]
    public Guid FourthPriorityId { get; set; }

    [Column("fifth_priority_id")]
    public Guid? FifthPriorityId { get; set; }

    [Column("sixth_priority_id")]
    public Guid? SixthPriorityId { get; set; }

    [Column("seventh_priority_id")]
    public Guid? SeventhPriorityId { get; set; }

    [Column("eighth_priority_id")]
    public Guid? EighthPriorityId { get; set; }

    [Column("first_alternative_id")]
    public Guid? FirstAlternativeId { get; set; }

    [Column("second_alternative_id")]
    public Guid? SecondAlternativeId { get; set; }

    [Column("third_alternative_id")]
    public Guid? ThirdAlternativeId { get; set; }

    [Column("first_avoid_id")]
    public Guid? FirstAvoidId { get; set; }

    [Column("second_avoid_id")]
    public Guid? SecondAvoidId { get; set; }

    [Column("third_avoid_id")]
    public Guid? ThirdAvoidId { get; set; }

    [ForeignKey(nameof(FirstPriorityId))]
    public RotationType FirstPriority { get; set; } = null!;

    [ForeignKey(nameof(SecondPriorityId))]
    public RotationType SecondPriority { get; set; } = null!;

    [ForeignKey(nameof(ThirdPriorityId))]
    public RotationType ThirdPriority { get; set; } = null!;

    [ForeignKey(nameof(FourthPriorityId))]
    public RotationType FourthPriority { get; set; } = null!;

    [ForeignKey(nameof(FifthPriorityId))]
    public RotationType? FifthPriority { get; set; }

    [ForeignKey(nameof(SixthPriorityId))]
    public RotationType? SixthPriority { get; set; }

    [ForeignKey(nameof(SeventhPriorityId))]
    public RotationType? SeventhPriority { get; set; }

    [ForeignKey(nameof(EighthPriorityId))]
    public RotationType? EighthPriority { get; set; }

    [ForeignKey(nameof(FirstAlternativeId))]
    public RotationType? FirstAlternative { get; set; }

    [ForeignKey(nameof(SecondAlternativeId))]
    public RotationType? SecondAlternative { get; set; }

    [ForeignKey(nameof(ThirdAlternativeId))]
    public RotationType? ThirdAlternative { get; set; }

    [ForeignKey(nameof(FirstAvoidId))]
    public RotationType? FirstAvoid { get; set; }

    [ForeignKey(nameof(SecondAvoidId))]
    public RotationType? SecondAvoid { get; set; }

    [ForeignKey(nameof(ThirdAvoidId))]
    public RotationType? ThirdAvoid { get; set; }

    [Column("additional_notes")]
    public string? AdditionalNotes { get; set; }
}