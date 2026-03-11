// ReSharper disable InconsistentNaming
namespace MedicalDemo.Models.DTO.Responses;

public class ResidentResponse
{
    public required string resident_id { get; set; }
    public required string first_name { get; set; }
    public required string last_name { get; set; }
    public int graduate_yr { get; set; }
    public required string email { get; set; }
    public required string phone_num { get; set; }
    public int weekly_hours { get; set; }
    public int total_hours { get; set; }
    public int bi_yearly_hours { get; set; }
    public int? hospital_role_profile { get; set; }
    public string? chief_type { get; set; }
}