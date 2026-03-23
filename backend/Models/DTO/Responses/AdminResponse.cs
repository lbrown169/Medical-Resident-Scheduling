using MedicalDemo.Models.Entities;
// ReSharper disable InconsistentNaming

namespace MedicalDemo.Models.DTO.Responses;

public class AdminResponse
{
    public required string admin_id { get; set; }
    public required string first_name { get; set; }
    public required string last_name { get; set; }
    public required string email { get; set; }
    public required string phone_num { get; set; }
}