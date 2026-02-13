using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Converters;

public static class RotationConverter
{
    private static readonly Dictionary<int, string> monthIndexToName = new()
    {
        { 0, "Jul"},
        { 1, "Aug"},
        { 2, "Sep"},
        { 3, "Oct"},
        { 4, "Nov"},
        { 5, "Dec"},
        { 6, "Jan"},
        { 7, "Feb"},
        { 8, "Mar"},
        { 9, "Apr"},
        { 10, "May"},
        { 11, "Jun"},
    };

    public static RotationResponse CreateRotationResponseFromModel(Rotation rotation)
    {
        return new()
        {
            RotationId = rotation.RotationId,
            ScheduleId = rotation.Pgy4RotationScheduleId,
            Month = rotation.Month,
            MonthIndex = rotation.MonthIndex,
            PgyYear = rotation.PgyYear,
            // Resident = new ResidentConverter().CreateResidentResponseFromResident(rotation.Resident),
            RotationType = RotationTypeConverter.CreateRotationTypeResponse(rotation.RotationType),
        };
    }

    public static Rotation CreateRotationFromResidentAndType(Resident resident, RotationType rotationType, Guid scheduleId, int monthIndex)
    {
        return new()
        {
            RotationId = Guid.NewGuid(),
            Pgy4RotationScheduleId = scheduleId,
            ResidentId = resident.ResidentId,
            MonthIndex = monthIndex,
            Month = monthIndexToName[monthIndex],
            PgyYear = 4,
            RotationType = rotationType,
            RotationTypeId = rotationType.RotationTypeId,
            Rotation1 = "",
        };
    }
}