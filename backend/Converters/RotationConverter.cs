using MedicalDemo.Enums;
using MedicalDemo.Extensions;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Converters;

public class RotationConverter(RotationTypeConverter rotationTypeConverter)
{
    private readonly RotationTypeConverter rotationTypeConverter = rotationTypeConverter;

    public RotationResponse CreateRotationResponseFromModel(Rotation rotation)
    {
        return new()
        {
            RotationId = rotation.RotationId,
            ResidentId = rotation.ResidentId,
            ScheduleId = rotation.Pgy4RotationScheduleId,
            Month = rotation.Month,
            AcademicYear = rotation.AcademicYear,
            AcademicMonthIndex = rotation.AcademicMonthIndex.ToAcademicIndex(),
            PgyYear = rotation.PgyYear,
            RotationType = rotationTypeConverter.CreateRotationTypeResponse(rotation.RotationType),
        };
    }

    public Rotation CreateRotationFromResidentAndType(
        string residentId,
        RotationType rotationType,
        Guid scheduleId,
        Guid rotationId,
        int academicYear,
        MonthOfYear calendarMonthIndex
    )
    {
        return new()
        {
            RotationId = rotationId,
            Pgy4RotationScheduleId = scheduleId,
            ResidentId = residentId,
            AcademicYear = academicYear,
            AcademicMonthIndex = (MonthOfYear)calendarMonthIndex.ToAcademicIndex(),
            Month = calendarMonthIndex.GetDisplayName(),
            PgyYear = 4,
            RotationType = rotationType,
            RotationTypeId = rotationType.RotationTypeId
        };
    }
}