// ReSharper disable InconsistentNaming
namespace MedicalDemo.Models.DTO.Requests;

public class ResidentUpdateRequest
{
    public string? first_name { get; set; }
    public string? last_name { get; set; }
    public int? graduate_yr { get; set; }
    public string? email { get; set; }
    public string? phone_num { get; set; }
    public int? hospital_role_profile { get; set; }
}