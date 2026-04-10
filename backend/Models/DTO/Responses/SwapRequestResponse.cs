namespace MedicalDemo.Models.DTO.Responses;

public class SwapRequestResponse
{
    public required Guid SwapRequestId { get; set; }
    public required Guid ScheduleId { get; set; }
    public required string RequesterId { get; set; }
    public required string RequesteeId { get; set; }
    public required DateOnly RequesterDate { get; set; }
    public required DateOnly RequesteeDate { get; set; }
    public required SwapRequestStatusResponse Status { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required DateTimeOffset UpdatedAt { get; set; }
    public required bool IsRead { get; set; }
    public required string? Details { get; set; }
}