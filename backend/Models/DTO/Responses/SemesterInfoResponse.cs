namespace MedicalDemo.Models.DTO.Responses;

using MedicalDemo.Enums;
using MedicalDemo.Extensions;

public class SemesterInfoResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public SemesterInfoResponse(Semester semester)
    {
        Id = (int)semester;
        Name = semester.GetDisplayName();
    }
}