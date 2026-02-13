using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
using RotationScheduleGenerator.Algorithm;

namespace MedicalDemo.Converters;

public static class RotationPrefRequestConverter
{
    public static RotationPrefResponse CreateRotationPrefResponseFromModel(
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
            request.FirstAlternative,
            request.SecondAlternative,
            request.ThirdAlternative,
        ]);

        return new RotationPrefResponse
        {
            RotationPrefRequestId = request.RotationPrefRequestId,
            Priorities = PrioritiesResponse,
            Alternatives = AlternativesResponse,
            Avoids = AvoidsResponse,
            AdditionalNotes = request.AdditionalNotes,
        };
    }

    public static RotationPrefRequest CreateModelFromRequestDto(RotationPrefRequestDto addRequestDto)
    {
        int prioritiesCount = addRequestDto.Priorities.Count;
        int alternativesCount = addRequestDto.Alternatives.Count;
        int avoidsCount = addRequestDto.Avoids.Count;

        RotationPrefRequest requestModel = new()
        {
            RotationPrefRequestId = Guid.NewGuid(),
            ResidentId = addRequestDto.ResidentId,

            FirstPriorityId = addRequestDto.Priorities[0],
            SecondPriorityId = addRequestDto.Priorities[1],
            ThirdPriorityId = addRequestDto.Priorities[2],
            FourthPriorityId = addRequestDto.Priorities[3],
            FifthPriorityId = prioritiesCount > 4 ? addRequestDto.Priorities[4] : null,
            SixthPriorityId = prioritiesCount > 5 ? addRequestDto.Priorities[5] : null,
            SeventhPriorityId = prioritiesCount > 6 ? addRequestDto.Priorities[6] : null,
            EighthPriorityId = prioritiesCount > 7 ? addRequestDto.Priorities[7] : null,

            FirstAlternativeId = alternativesCount > 0 ? addRequestDto.Alternatives[0] : null,
            SecondAlternativeId = alternativesCount > 1 ? addRequestDto.Alternatives[1] : null,
            ThirdAlternativeId = alternativesCount > 2 ? addRequestDto.Alternatives[2] : null,

            FirstAvoidId = avoidsCount > 0 ? addRequestDto.Avoids[0] : null,
            SecondAvoidId = avoidsCount > 1 ? addRequestDto.Avoids[1] : null,
            ThirdAvoidId = avoidsCount > 2 ? addRequestDto.Avoids[2] : null,

            AdditionalNotes = addRequestDto.AdditionalNotes,
        };

        return requestModel;
    }

    public static void UpdateModelFromUpdateRequest(RotationPrefRequest prefRequestModel, UpdateRotationPrefRequest updateRequest)
    {
        int prioritiesCount = updateRequest.Priorities.Count;
        int alternativesCount = updateRequest.Alternatives.Count;
        int avoidsCount = updateRequest.Avoids.Count;

        prefRequestModel.FirstPriorityId = updateRequest.Priorities[0];
        prefRequestModel.SecondPriorityId = updateRequest.Priorities[1];
        prefRequestModel.ThirdPriorityId = updateRequest.Priorities[2];
        prefRequestModel.FourthPriorityId = updateRequest.Priorities[3];
        prefRequestModel.FifthPriorityId = prioritiesCount > 4 ? updateRequest.Priorities[4] : null;
        prefRequestModel.SixthPriorityId = prioritiesCount > 5 ? updateRequest.Priorities[5] : null;
        prefRequestModel.SeventhPriorityId =
            prioritiesCount > 6 ? updateRequest.Priorities[6] : null;
        prefRequestModel.EighthPriorityId =
            prioritiesCount > 7 ? updateRequest.Priorities[7] : null;

        prefRequestModel.FirstAlternativeId =
            alternativesCount > 0 ? updateRequest.Alternatives[0] : null;
        prefRequestModel.SecondAlternativeId =
            alternativesCount > 1 ? updateRequest.Alternatives[1] : null;
        prefRequestModel.ThirdAlternativeId =
            alternativesCount > 2 ? updateRequest.Alternatives[2] : null;

        prefRequestModel.FirstAvoidId = avoidsCount > 0 ? updateRequest.Avoids[0] : null;
        prefRequestModel.SecondAvoidId = avoidsCount > 1 ? updateRequest.Avoids[1] : null;
        prefRequestModel.ThirdAvoidId = avoidsCount > 2 ? updateRequest.Avoids[2] : null;

        prefRequestModel.AdditionalNotes = updateRequest.AdditionalNotes;
    }

    public static AlgorithmRotationPrefRequest CreateAlgorithmSchedulePrefRequestFromModel(
        RotationPrefRequest requestModel
    )
    {
        AlgorithmRotationType[] AlgorithmPriorities = [.. FilterNullAndConvertToAlgorithmType([
            requestModel.FirstPriority,
            requestModel.SecondPriority,
            requestModel.ThirdPriority,
            requestModel.FourthPriority,
            requestModel.FifthPriority,
            requestModel.SixthPriority,
            requestModel.SeventhPriority,
            requestModel.EighthPriority,
        ])];

        AlgorithmRotationType[] AlgorithmAlternatives = [.. FilterNullAndConvertToAlgorithmType([
            requestModel.FirstAlternative,
            requestModel.SecondAlternative,
            requestModel.ThirdAlternative,
        ])];

        AlgorithmRotationType[] AlgorithmAvoids = [.. FilterNullAndConvertToAlgorithmType([
            requestModel.FirstAlternative,
            requestModel.SecondAlternative,
            requestModel.ThirdAlternative,
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

    private static List<RotationTypeResponse> FilterNullAndConvertToResponse(
        List<RotationType?> rotationTypes
    )
    {
        return
        [
            .. rotationTypes
                .Where((avoid) => avoid != null)
                .Cast<RotationType>()
                .Select(RotationTypeConverter.CreateRotationTypeResponse),
        ];
    }

    private static List<AlgorithmRotationType> FilterNullAndConvertToAlgorithmType(List<RotationType?> rotationTypes)
    {
        return
        [
            .. rotationTypes
                .Where((avoid) => avoid != null)
                .Cast<RotationType>()
                .Select(RotationTypeConverter.CreateAlgorithmRotationTypeFromModel),
        ];
    }
}

