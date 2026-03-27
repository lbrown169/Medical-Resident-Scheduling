using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using PostmarkDotNet.Model;

namespace MedicalDemo.Models;

public class ConstraintResult
{
    public bool IsViolated { get;}
    public string? Message { get; }
    public bool? IsOverridable { get; }

    private ConstraintResult(bool isViolated, string? message, bool? isOverridable)
    {
        IsViolated = isViolated;
        Message = message;
        IsOverridable = isOverridable;
    }

    public static ConstraintResult NoViolation() => new(false, null, null);
    public static ConstraintResult Violation(string message, bool isOverridable) => new(true, message, isOverridable);

    [MemberNotNullWhen(true, nameof(Message), nameof(IsOverridable))]
    public bool HasViolation() => IsViolated;
}