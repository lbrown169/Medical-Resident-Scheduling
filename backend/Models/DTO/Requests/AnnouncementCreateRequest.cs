// ReSharper disable InconsistentNaming

using System.ComponentModel.DataAnnotations;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Models.DTO.Requests;

public class AnnouncementCreateRequest
{
    [Required]
    public required string message { get; set; }

    // TODO: This should not be accepted through body, but instead as logged in user
    public string? authorId { get; set; }
}