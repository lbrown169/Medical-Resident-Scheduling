using MedicalDemo.Models.Entities;

namespace MedicalDemo.Models.DTO.Responses;

public class RotationPrefResponse
{
    public Guid RotationPrefRequestId { get; set; }

    public List<RotationTypeResponse> Priorities { get; set; } = [];

    public List<RotationTypeResponse> Alternatives { get; set; } = [];

    public List<RotationTypeResponse> Avoids { get; set; } = [];

    public string? AdditionalNotes { get; set; } = null!;
}