using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MedicalDemo.Enums;

namespace MedicalDemo.Models.DTO.Requests;

public class Pgy4UpdateResidentChiefTypeRequest
{
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ChiefType ChiefType { get; set; }
}