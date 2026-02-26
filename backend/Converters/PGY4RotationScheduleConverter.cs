using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Converters;

public class Pgy4RotationScheduleConverter(RotationConverter rotationConverter, ResidentConverter residentConverter)
{
    private readonly RotationConverter rotationConverter = rotationConverter;
    private readonly ResidentConverter residentConverter = residentConverter;

    public Pgy4RotationScheduleResponse CreateRotationScheduleResponseFromModel(
        Pgy4RotationSchedule scheduleModel
    )
    {
        Dictionary<string, List<Rotation>> residentIdToRotations = [];

        foreach (Rotation rotation in scheduleModel.Rotations)
        {
            if (residentIdToRotations.TryGetValue(rotation.ResidentId, out List<Rotation>? value))
            {
                value.Add(rotation);
            }
            else
            {
                residentIdToRotations.Add(rotation.ResidentId, []);
                residentIdToRotations[rotation.ResidentId].Add(rotation);
            }
        }

        List<Pgy4ResidentRotationScheduleResponse> residentScheduleResponses =
        [
            .. residentIdToRotations.Select(
                (kvp) =>
                {
                    Resident resident = kvp.Value[0].Resident;
                    return CreateSingleResidentRotationSchedule(resident, kvp.Value);
                }
            ),
        ];

        return new()
        {
            Pgy4RotationScheduleId = scheduleModel.Pgy4RotationScheduleId,
            Seed = scheduleModel.Seed,
            ResidentCount = residentIdToRotations.Count,
            Year = scheduleModel.Year,
            IsPublished = scheduleModel.IsPublished,
            Schedule = residentScheduleResponses,
        };
    }

    public Pgy4ResidentRotationScheduleResponse CreateSingleResidentRotationSchedule(
        Resident resident,
        List<Rotation> rotations
    )
    {
        return new()
        {
            Resident = residentConverter.CreateResidentResponseFromResident(resident),
            Rotations =
            [
                .. rotations
                    .Select(rotationConverter.CreateRotationResponseFromModel)
                    .OrderBy((rotationRes) => rotationRes.MonthIndex),
            ],
        };
    }

    public Pgy4RotationSchedulesListResponse CreateRotationSchedulesListResponseFromModel(
        List<Pgy4RotationSchedule> scheduleModel
    )
    {
        return new()
        {
            Count = scheduleModel.Count,
            Schedules = [.. scheduleModel.Select(CreateRotationScheduleResponseFromModel)],
        };
    }
}