using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Converters;

public class RotationPrefSubmissionWindowConverter
{
    public RotationPrefSubmissionWindow CreateModelFromRequest(
        RotationPrefSubmissionWindowRequest request,
        int academicYear
    )
    {
        return new()
        {
            AcademicYear = academicYear,
            AvailableDate = request.AvailableDate,
            DueDate = request.DueDate,
        };
    }

    public RotationPrefSubmissionWindowResponse CreateResponseFromModel(
        RotationPrefSubmissionWindow model
    )
    {
        return new()
        {
            AcademicYear = model.AcademicYear,
            AvailableDate = model.AvailableDate,
            DueDate = model.DueDate,
        };
    }
}