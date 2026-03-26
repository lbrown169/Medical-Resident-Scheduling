namespace MedicalDemo.Models.DTO.Responses;

public class RotationPrefSubmissionWindowResponse
{
    public int AcademicYear { get; set; }

    public DateTime? AvailableDate { get; set; }

    public DateTime? DueDate { get; set; }
}