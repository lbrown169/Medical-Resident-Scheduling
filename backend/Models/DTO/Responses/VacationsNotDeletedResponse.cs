// ReSharper disable InconsistentNaming
namespace MedicalDemo.Models.DTO.Responses;

public class VacationsNotDeletedResponse
{
    public required List<Guid> notDeleted { get; set; }
}