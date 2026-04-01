namespace MedicalDemo.Models.DTO.Scheduling;

public record HospitalRole(
    string Name,
    bool DoesShort,
    bool DoesLong,
    bool DoesTrainingShort,
    bool DoesTrainingLong
)
{
    public static readonly HospitalRole Unassigned = new("Unassigned", true, true, true, true);
}