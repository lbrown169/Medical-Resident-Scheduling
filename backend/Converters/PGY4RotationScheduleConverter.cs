using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Converters;

public static class PGY4RotationScheduleConverter
{
    public static PGY4RotationScheduleResponse CreateRotationScheduleResponseFromModel(
        PGY4RotationSchedule scheduleModel
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

        List<PGY4ResidenRotationScheduleResponse> residentScheduleResponses =
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
            PGY4RotationScheduleId = scheduleModel.PGY4RotationScheduleId,
            Seed = scheduleModel.Seed,
            ResidentCount = residentIdToRotations.Count,
            Year = scheduleModel.Year,
            IsPublished = scheduleModel.IsPublished,
            Schedule = residentScheduleResponses,
        };
    }

    public static PGY4ResidenRotationScheduleResponse CreateSingleResidentRotationSchedule(
        Resident resident,
        List<Rotation> rotations
    )
    {
        return new()
        {
            Resident = new ResidentConverter().CreateResidentResponseFromResident(resident),
            Rotations =
            [
                .. rotations
                    .Select(RotationConverter.CreateRotationResponseFromModel)
                    .OrderBy((rotationRes) => rotationRes.MonthIndex),
            ],
        };
    }

    public static PGY4RotationSchedulesListResponse CreateRotationSchedulesListResponseFromModel(
        List<PGY4RotationSchedule> scheduleModel
    )
    {
        return new()
        {
            Count = scheduleModel.Count,
            Schedules = [.. scheduleModel.Select(CreateRotationScheduleResponseFromModel)],
        };
    }
}
