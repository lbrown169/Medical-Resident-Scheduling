using MedicalDemo.Enums;

namespace MedicalDemo.Models.DTO.Pgy4Scheduling;

public class Pgy4ConstraintViolation
{
    public Pgy4ConstraintType ConstraintViolated { get; set; }

    public List<Pgy4ConstraintError> Errors { get; set; } = null!;
}