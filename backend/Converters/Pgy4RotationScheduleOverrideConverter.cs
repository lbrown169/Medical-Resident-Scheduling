using MedicalDemo.Enums;
using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Converters;

public class Pgy4RotationScheduleOverrideConverter(
    ResidentConverter residentConverter,
    RotationTypeConverter rotationTypeConverter,
    Pgy4RotationScheduleConverter pgy4RotationScheduleConverter
)
{
    private readonly ResidentConverter residentConverter = residentConverter;
    private readonly RotationTypeConverter rotationTypeConverter = rotationTypeConverter;
    private readonly Pgy4RotationScheduleConverter pgy4RotationScheduleConverter =
        pgy4RotationScheduleConverter;

    public Pgy4RotationScheduleOverride CreateModelFromRequest(
        Guid rotationScheduleId,
        Pgy4RotationScheduleOverrideRequest request
    )
    {
        return new()
        {
            Pgy4RotationScheduleOverrideId = Guid.NewGuid(),
            Pgy4RotationScheduleId = rotationScheduleId,
            ResidentOverrideId = request.ResidentId,
            AcademicMonthIndexOverride = (MonthOfYear)request.AcademicMonthIndex,
            RotationTypeOverrideId = request.newRotationTypeId,
        };
    }

    public Pgy4RotationScheduleOverrideResponse CreateResponseFromModel(
        Pgy4RotationScheduleOverride model
    )
    {
        return new()
        {
            Pgy4RotationScheduleOverrideId = model.Pgy4RotationScheduleOverrideId,
            Pgy4RotationScheduleId = model.Pgy4RotationScheduleId,
            AcademicMonthIndex = model.AcademicMonthIndexOverride,
            Resident = residentConverter.CreateResidentResponseFromResident(model.Resident),
            OverrideRotation = rotationTypeConverter.CreateRotationTypeResponse(model.RotationType),
        };
    }

    public Pgy4RotationScheduleOverrideListResponse CreateListResponseFromModels(
        Pgy4RotationSchedule schedule
    )
    {
        return new()
        {
            Count = schedule.Overrides.Count,
            Schedule = pgy4RotationScheduleConverter.CreateRotationScheduleResponseFromModel(
                schedule
            ),
            Overrides = [.. schedule.Overrides.Select(CreateResponseFromModel)],
        };
    }

    public void UpdateScheduleWithOverrides(
        Pgy4RotationSchedule schedule,
        List<Pgy4RotationScheduleOverride> overrides
    )
    {
        // Populate residentId to overrides dictionary
        Dictionary<string, List<Pgy4RotationScheduleOverride>> residentIdToOverrides = [];
        foreach (Pgy4RotationScheduleOverride scheduleOverride in overrides)
        {
            if (
                residentIdToOverrides.TryGetValue(
                    scheduleOverride.ResidentOverrideId,
                    out List<Pgy4RotationScheduleOverride>? value
                )
            )
            {
                value.Add(scheduleOverride);
            }
            else
            {
                residentIdToOverrides.Add(scheduleOverride.ResidentOverrideId, []);
                residentIdToOverrides[scheduleOverride.ResidentOverrideId].Add(scheduleOverride);
            }
        }

        // Apply overrides to the original schedule
        foreach (Rotation rotation in schedule.Rotations)
        {
            if (
                residentIdToOverrides.TryGetValue(
                    rotation.ResidentId,
                    out List<Pgy4RotationScheduleOverride>? overridesForResident
                )
            )
            {
                Pgy4RotationScheduleOverride? foundOverride = overridesForResident.FirstOrDefault(
                    (o) =>
                        o.ResidentOverrideId == rotation.ResidentId
                        && o.AcademicMonthIndexOverride == rotation.AcademicMonthIndex
                );

                if (foundOverride != null)
                {
                    rotation.RotationType = foundOverride.RotationType;
                    rotation.RotationTypeId = foundOverride.RotationTypeOverrideId;
                }
            }
        }
    }

    public void UpdateResidentScheduleWithOverrides(
        List<Rotation> residentSchedule,
        List<Pgy4RotationScheduleOverride> overrides
    )
    {
        residentSchedule = [.. residentSchedule.OrderBy((rotation) => rotation.AcademicMonthIndex)];
        overrides = [.. overrides.OrderBy((o) => o.AcademicMonthIndexOverride)];

        int overrideIndex = 0;
        foreach (Rotation rotation in residentSchedule)
        {
            if (
                overrideIndex < overrides.Count
                && overrides[overrideIndex].AcademicMonthIndexOverride
                    == rotation.AcademicMonthIndex
                && overrides[overrideIndex].ResidentOverrideId == rotation.ResidentId
            )
            {
                rotation.RotationType = overrides[overrideIndex].RotationType;
                rotation.RotationTypeId = overrides[overrideIndex].RotationTypeOverrideId;
            }
        }
    }
}
