using MedicalDemo.Algorithms.Pgy4RotationScheduleGenerator;
using MedicalDemo.Enums;
using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Converters;

public class RotationPrefRequestConverter(RotationTypeConverter rotationTypeConverter, ResidentConverter residentConverter)
{
    private readonly RotationTypeConverter rotationTypeConverter = rotationTypeConverter;
    private readonly ResidentConverter residentConverter = residentConverter;

    public RotationPrefResponse CreateRotationPrefResponseFromModel(
        RotationPrefRequest request
    )
    {
        List<RotationTypeResponse> PrioritiesResponse = FilterNullAndConvertToResponse([
            request.FirstPriority,
            request.SecondPriority,
            request.ThirdPriority,
            request.FourthPriority,
            request.FifthPriority,
            request.SixthPriority,
            request.SeventhPriority,
            request.EighthPriority,
        ]);

        List<RotationTypeResponse> AlternativesResponse = FilterNullAndConvertToResponse([
            request.FirstAlternative,
            request.SecondAlternative,
            request.ThirdAlternative,
        ]);

        List<RotationTypeResponse> AvoidsResponse = FilterNullAndConvertToResponse([
            request.FirstAvoid,
            request.SecondAvoid,
            request.ThirdAvoid,
        ]);

        return new RotationPrefResponse
        {
            RotationPrefRequestId = request.RotationPrefRequestId,
            Resident = residentConverter.CreateResidentResponseFromResident(request.Resident),
            Priorities = PrioritiesResponse,
            Alternatives = AlternativesResponse,
            Avoids = AvoidsResponse,
            AdditionalNotes = request.AdditionalNotes,
        };
    }

    public RotationPrefRequest CreateModelFromRequestDto(
        RotationPrefRequestDto addRequestDto
    )
    {
        RotationPrefRequest requestModel = new()
        {
            RotationPrefRequestId = Guid.NewGuid(),
            ResidentId = addRequestDto.ResidentId,
        };

        UpdateModelFromRotationTypes(
            requestModel,
            addRequestDto.Priorities,
            addRequestDto.Alternatives,
            addRequestDto.Avoids,
            addRequestDto.AdditionalNotes
        );

        return requestModel;
    }

    public void UpdateModelFromUpdateRequest(
        RotationPrefRequest prefRequestModel,
        UpdateRotationPrefRequest updateRequest
    )
    {
        UpdateModelFromRotationTypes(
            prefRequestModel,
            updateRequest.Priorities,
            updateRequest.Alternatives,
            updateRequest.Avoids,
            updateRequest.AdditionalNotes
        );
    }

    public AlgorithmRotationPrefRequest CreateAlgorithmSchedulePrefRequestFromModel(
        RotationPrefRequest requestModel
    )
    {
        Pgy4RotationTypeEnum[] AlgorithmPriorities = [.. FilterNullAndConvertToAlgorithmType([
            requestModel.FirstPriority,
            requestModel.SecondPriority,
            requestModel.ThirdPriority,
            requestModel.FourthPriority,
            requestModel.FifthPriority,
            requestModel.SixthPriority,
            requestModel.SeventhPriority,
            requestModel.EighthPriority,
        ])];

        Pgy4RotationTypeEnum[] AlgorithmAlternatives = [.. FilterNullAndConvertToAlgorithmType([
            requestModel.FirstAlternative,
            requestModel.SecondAlternative,
            requestModel.ThirdAlternative,
        ])];

        Pgy4RotationTypeEnum[] AlgorithmAvoids = [.. FilterNullAndConvertToAlgorithmType([
            requestModel.FirstAvoid,
            requestModel.SecondAvoid,
            requestModel.ThirdAvoid,
        ])];

        return new()
        {
            RotationPrefRequestId = requestModel.RotationPrefRequestId,
            Requester = requestModel.Resident,
            Priorities = AlgorithmPriorities,
            Alternatives = AlgorithmAlternatives,
            Avoids = AlgorithmAvoids
        };
    }

    private static void UpdateModelFromRotationTypes(
        RotationPrefRequest requestModel,
        List<Guid> priorities,
        List<Guid> alternatives,
        List<Guid> avoids,
        string? additionalNotes
    )
    {
        int prioritiesCount = priorities.Count;
        int alternativesCount = alternatives.Count;
        int avoidsCount = avoids.Count;

        requestModel.FirstPriorityId = priorities[0];
        requestModel.SecondPriorityId = priorities[1];
        requestModel.ThirdPriorityId = priorities[2];
        requestModel.FourthPriorityId = priorities[3];
        requestModel.FifthPriorityId = prioritiesCount > 4 ? priorities[4] : null;
        requestModel.SixthPriorityId = prioritiesCount > 5 ? priorities[5] : null;
        requestModel.SeventhPriorityId = prioritiesCount > 6 ? priorities[6] : null;
        requestModel.EighthPriorityId = prioritiesCount > 7 ? priorities[7] : null;

        requestModel.FirstAlternativeId = alternativesCount > 0 ? alternatives[0] : null;
        requestModel.SecondAlternativeId = alternativesCount > 1 ? alternatives[1] : null;
        requestModel.ThirdAlternativeId = alternativesCount > 2 ? alternatives[2] : null;

        requestModel.FirstAvoidId = avoidsCount > 0 ? avoids[0] : null;
        requestModel.SecondAvoidId = avoidsCount > 1 ? avoids[1] : null;
        requestModel.ThirdAvoidId = avoidsCount > 2 ? avoids[2] : null;

        requestModel.AdditionalNotes = additionalNotes;
    }

    private List<RotationTypeResponse> FilterNullAndConvertToResponse(
        List<RotationType?> rotationTypes
    )
    {
        return
        [
            .. rotationTypes
                .Where((avoid) => avoid != null)
                .Cast<RotationType>()
                .Select(rotationTypeConverter.CreateRotationTypeResponse),
        ];
    }

    private List<Pgy4RotationTypeEnum> FilterNullAndConvertToAlgorithmType(List<RotationType?> rotationTypes)
    {
        return
        [
            .. rotationTypes
                .Where((avoid) => avoid != null)
                .Cast<RotationType>()
                .Select(rotationTypeConverter.ConvertRotationTypeModelToEnum),
        ];
    }
}