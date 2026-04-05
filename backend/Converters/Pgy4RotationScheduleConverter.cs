using MedicalDemo.Enums;
using MedicalDemo.Extensions;
using MedicalDemo.Models.DTO.Pgy4Scheduling;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Converters;

public class Pgy4RotationScheduleConverter(
    RotationConverter rotationConverter,
    ResidentConverter residentConverter,
    RotationTypeConverter rotationTypeConverter
)
{
    private readonly RotationConverter rotationConverter = rotationConverter;
    private readonly ResidentConverter residentConverter = residentConverter;
    private readonly RotationTypeConverter rotationTypeConverter = rotationTypeConverter;

    public Pgy4RotationScheduleResponse CreateRotationScheduleResponseFromModel(
        Pgy4RotationSchedule scheduleModel
    )
    {
        Dictionary<string, List<Rotation>> residentIdToRotations = [];

        foreach (Rotation rotation in scheduleModel.Rotations)
        {
            if (rotation.ResidentId == null)
            {
                continue;
            }

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
                    Resident resident = kvp.Value[0].Resident!;
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
                    .OrderBy((rotationRes) => rotationRes.AcademicMonthIndex),
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

    public Pgy4ScheduleConstraintErrorResponse CreateConstraintErrorResponseFromModel(
        Pgy4ConstraintError error,
        Dictionary<string, Resident> residentIdToResident
    )
    {
        Resident? foundResident = null;

        if (
            error.Resident != null
            && residentIdToResident.TryGetValue(error.Resident.ResidentId, out Resident? value)
        )
        {
            foundResident = value;
        }

        return new()
        {
            Message = error.Message,
            Resident =
                foundResident != null
                    ? residentConverter.CreateResidentResponseFromResident(foundResident)
                    : null,
            AcademicMonthIndex =
                error.MonthIndex != null
                    ? ((MonthOfYear)error.MonthIndex).ToAcademicIndex() // Explicit conversion here to convert MonthOfYear? to MonthOfYear
                    : null,
        };
    }

    public Pgy4ScheduleConstraintViolationResponse CreateConstraintViolationResponseFromModel(
        Pgy4ConstraintViolation violation,
        Dictionary<string, Resident> residentIdToResident
    )
    {
        return new()
        {
            ConstraintViolated = violation.ConstraintViolated.GetDisplayName(),
            Errors =
            [
                .. violation.Errors.Select(
                    (e) => CreateConstraintErrorResponseFromModel(e, residentIdToResident)
                ),
            ],
        };
    }

    public Pgy4ScheduleConstraintViolationsListResponse CreateViolationsListResponse(
        Pgy4RotationSchedule schedule,
        List<Pgy4ConstraintViolation> violations
    )
    {
        Dictionary<string, Resident> residentIdToResident = [];
        foreach (Rotation rotation in schedule.Rotations)
        {
            if (
                rotation.ResidentId != null
                && rotation.Resident != null
                && !residentIdToResident.ContainsKey(rotation.ResidentId)
            )
            {
                residentIdToResident.Add(rotation.ResidentId, rotation.Resident);
            }
        }

        return new()
        {
            Schedule = CreateRotationScheduleResponseFromModel(schedule),
            Violations =
            [
                .. violations
                    .Where((violation) => violation.Errors.Count > 0)
                    .Select(
                        (violation) =>
                            CreateConstraintViolationResponseFromModel(
                                violation,
                                residentIdToResident
                            )
                    ),
            ],
        };
    }

    public Pgy4ScheduleData CreateAlgorithmScheduleDataFromModel(Pgy4RotationSchedule scheduleModel)
    {
        Dictionary<string, List<Rotation>> residentIdToRotations = [];
        Dictionary<string, Resident> residentIdToResident = [];
        foreach (Rotation rotation in scheduleModel.Rotations)
        {
            if (rotation.ResidentId == null || rotation.Resident == null)
            {
                continue;
            }

            if (residentIdToRotations.TryGetValue(rotation.ResidentId, out List<Rotation>? value))
            {
                value.Add(rotation);
            }
            else
            {
                residentIdToRotations.Add(rotation.ResidentId, []);
                residentIdToRotations[rotation.ResidentId].Add(rotation);
            }

            if (!residentIdToResident.ContainsKey(rotation.ResidentId))
            {
                residentIdToResident.Add(rotation.ResidentId, rotation.Resident);
            }
        }

        Dictionary<AlgorithmResident, Pgy4RotationTypeEnum[]> algoSchedule = [];
        foreach (KeyValuePair<string, List<Rotation>> kvp in residentIdToRotations)
        {
            Resident resident = residentIdToResident[kvp.Key];
            AlgorithmResident algorithmResident = new()
            {
                ResidentId = resident.ResidentId,
                FirstName = resident.FirstName,
                LastName = resident.LastName,
                ChiefType = resident.ChiefType,
            };

            // Sort rotations by calendar year
            List<Rotation> sortedRotation =
            [
                .. kvp.Value.OrderBy((rotation) => rotation.RotationMonthOfYear),
            ];
            algoSchedule.Add(
                algorithmResident,
                [
                    .. sortedRotation.Select(rotation =>
                        rotationTypeConverter.ConvertRotationTypeModelToEnum(rotation.RotationType)
                    ),
                ]
            );
        }

        return new() { Schedule = algoSchedule };
    }
}