namespace MedicalDemo.Models.DTO.Responses;

public class RotationTypeResponse
{
    public Guid RotationTypeId { get; set; }

    public string RotationName { get; set; } = null!;

    public bool DoesLongCall { get; set; }

    public bool DoesShortCall { get; set; }

    public bool DoesTrainingLongCall { get; set; }

    public bool DoesTrainingShortCall { get; set; }

    public bool IsChiefRotation { get; set; }

    public List<int> PgyYears { get; set; } = [];
}