using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalDemo.Models.Entities;

[Table("rotation_pref_request")]
public class RotationPrefRequest
{
    [Key]
    public Guid RotationPrefRequestId { get; set; }

    [ForeignKey(nameof(Resident))]
    public string ResidentId { get; set; } = null!;

    [ForeignKey(nameof(ResidentId))]
    public Resident Resident { get; set; } = null!;

    [Required]
    public Guid FirstPriorityId { get; set; }

    [Required]
    public Guid SecondPriorityId { get; set; }

    [Required]
    public Guid ThirdPriorityId { get; set; }

    [Required]
    public Guid FourthPriorityId { get; set; }

    public Guid? FifthPriorityId { get; set; }

    public Guid? SixthPriorityId { get; set; }

    public Guid? SeventhPriorityId { get; set; }

    public Guid? EighthPriorityId { get; set; }

    public Guid? FirstAlternativeId { get; set; }

    public Guid? SecondAlternativeId { get; set; }

    public Guid? ThirdAlternativeId { get; set; }

    public Guid? FirstAvoidId { get; set; }

    public Guid? SecondAvoidId { get; set; }

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

    public string? AdditionalNotes { get; set; }
}