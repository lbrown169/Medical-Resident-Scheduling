using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Converters;

public static class PGY4RotationScheduleConverter
{
    // public static PGY4RotationScheduleResponse CreateRotationScheduleResponseFromModel(
    //     Dictionary<Resident, Rotation[]> scheduleModel
    // )
    // {
    //     List<PGY4ResidenRotationScheduleResponse> scheduleResponse =
    //     [
    //         .. scheduleModel.Select(
    //             (kvp) =>
    //             {
    //                 return new PGY4ResidenRotationScheduleResponse()
    //                 {
    //                     ResidentResponse =
    //                         new ResidentConverter().CreateResidentResponseFromResident(kvp.Key),
    //                     Rotations =
    //                     [
    //                         .. kvp.Value.Select(
    //                             (rotation) =>
    //                                 RotationConverter.CreateRotationResponseFromModel(rotation)
    //                         ),
    //                     ],
    //                 };
    //             }
    //         ),
    //     ];

    //     return new() { ResidentCount = scheduleModel.Count, Schedule = scheduleResponse };
    // }

    public static PGY4RotationScheduleResponse CreateRotationScheduleResponseFromModel(PGY4RotationSchedule scheduleModel)
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

        List<PGY4ResidenRotationScheduleResponse> residentScheduleResponses = [.. residentIdToRotations.Select((kvp) =>
        {
            Resident resident = kvp.Value[0].Resident;

            return new PGY4ResidenRotationScheduleResponse()
            {
                Resident = new ResidentConverter().CreateResidentResponseFromResident(resident),
                Rotations = [.. kvp.Value.Select(RotationConverter.CreateRotationResponseFromModel)]
            };
        })];

        return new()
        {
            ScheduleId = scheduleModel.PGY4RotationScheduleId,
            ResidentCount = residentIdToRotations.Count,
            Schedule = residentScheduleResponses,
        };
    }

    public static PGY4RotationSchedulesListResponse CreateRotationSchedulesListResponseFromModel(List<PGY4RotationSchedule> scheduleModel)
    {
        return new()
        {
            Count = scheduleModel.Count,
            Schedules = [.. scheduleModel.Select(CreateRotationScheduleResponseFromModel)]
        };
    }
}
