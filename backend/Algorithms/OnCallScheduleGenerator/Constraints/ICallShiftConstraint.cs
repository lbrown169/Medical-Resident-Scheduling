using MedicalDemo.Enums;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Scheduling;

namespace MedicalDemo.Algorithms.OnCallScheduleGenerator.Constraints;

public interface ICallShiftConstraint
{
    ConstraintResult Evaluate(ResidentDto resident, DateOnly date, CallShiftType shiftType);
}