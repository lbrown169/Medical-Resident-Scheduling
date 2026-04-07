namespace MedicalDemo.Models.DTO.Responses;

public class ViolationResultResponse
{
    public bool IsViolation { get; }
    public bool? IsOverridable { get; }
    public List<ConstraintResult> Violations { get; }
    public bool IsAllowed { get; set; } // indicates if there is authority for available overrides

    public ViolationResultResponse(ViolationResult result, bool adminOverride = false)
    {
        IsViolation = result.IsViolation;
        IsOverridable = result.IsOverridable;
        Violations = result.Violations;
        // if violation is overridable but have no adminOverride priviledges -> not allowed
        IsAllowed = !IsViolation ||(adminOverride && IsOverridable == true);
    }

    public ViolationResultResponse() { }
}