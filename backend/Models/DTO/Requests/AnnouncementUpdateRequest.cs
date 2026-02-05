using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Models.DTO.Requests;

public class AnnouncementUpdateRequest
{
    public string? Message { get; set; }
    public string? AuthorId { get; set; }
}