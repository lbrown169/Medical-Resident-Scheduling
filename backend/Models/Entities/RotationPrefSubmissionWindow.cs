using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalDemo.Models.Entities;

[Table("rotation_pref_request_submission_window")]
public class RotationPrefSubmissionWindow
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int AcademicYear { get; set; }

    public DateTime? AvailableDate { get; set; }

    public DateTime? DueDate { get; set; }
}