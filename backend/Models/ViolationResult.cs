namespace MedicalDemo.Models;

public class ViolationResult
{
    public bool IsViolation { get; }
    public bool? IsOverridable { get; }
    public List<ConstraintResult> Violations { get; }

    public ViolationResult(List<ConstraintResult> violations)
    {
        Violations = violations;
        IsViolation = Violations.Any(v => v.IsViolated);
        IsOverridable = IsViolation ? null : Violations.All(v => v.IsOverridable == true);
    }
}