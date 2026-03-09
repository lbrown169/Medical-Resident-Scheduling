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
            ScheduleId = rotation.Pgy4RotationScheduleId,
            Month = rotation.Month,
            AcademicMonthIndex = rotation.AcademicMonthIndex,
            PgyYear = rotation.PgyYear,
            RotationType = rotationTypeConverter.CreateRotationTypeResponse(rotation.RotationType),
        };
    }

    public Rotation CreateRotationFromResidentAndType(string residentId, RotationType rotationType, Guid scheduleId, MonthOfYear calendarMonthIndex)
    {
        return new()
        {
            RotationId = Guid.NewGuid(),
            Pgy4RotationScheduleId = scheduleId,
            ResidentId = residentId,
            AcademicMonthIndex = (MonthOfYear)calendarMonthIndex.ToAcademicIndex(),
            Month = calendarMonthIndex.GetDisplayName(),
            PgyYear = 4,
            RotationType = rotationType,
            RotationTypeId = rotationType.RotationTypeId,
            Rotation1 = "",
        };
    }
}