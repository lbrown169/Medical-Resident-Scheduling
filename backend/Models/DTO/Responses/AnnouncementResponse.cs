// ReSharper disable InconsistentNaming
namespace MedicalDemo.Models.DTO.Responses;

public class AnnouncementResponse
{
    public required string announcementId { get; set; }
    public required string message { get; set; }
    public required string createdAt { get; set; }
}