namespace MedicalDemo.Models.DTO.Responses;

public class SwapRequestsNotDeletedResponse
{
    public required List<Guid> notDeleted { get; set; }
}