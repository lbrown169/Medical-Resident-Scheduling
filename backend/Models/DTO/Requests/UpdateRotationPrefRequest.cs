using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Models.DTO.Requests;

public class UpdateRotationPrefRequest
{
    [MaxLength(8, ErrorMessage = "Maximum of 8 priorities allowed")]
    [MinLength(4, ErrorMessage = "At least 4 priorities are required")]
    public List<Guid> Priorities { get; set; } = [];

    [MaxLength(3, ErrorMessage = "Maximum of 3 alternatives allowed")]
    public List<Guid> Alternatives { get; set; } = [];

    [MaxLength(3, ErrorMessage = "Maximum of 3 avoids allowed")]
    public List<Guid> Avoids { get; set; } = [];

    public string? AdditionalNotes { get; set; } = null!;
}