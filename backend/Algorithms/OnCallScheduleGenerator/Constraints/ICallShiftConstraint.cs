using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Scheduling;

namespace MedicalDemo.Interfaces;

public interface ICallShiftConstraint
{
    ConstraintResult Evaluate(ResidentDto resident, DateOnly date);
}