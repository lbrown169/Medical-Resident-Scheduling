namespace MedicalDemo.Models.DTO.Responses;

public class ConstraintResultResponse
{
    public bool IsViolated { get; }
    public string? Message { get; }
    public bool? IsOverridable { get; }
}