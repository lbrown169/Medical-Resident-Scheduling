using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Converters;

public static class RotationPrefRequestConverter
{
    public static RotationPrefResponse CreateRotationPrefResponseFromModel(
        RotationPrefRequest request
    )
    {
        List<RotationType?> Priorities =
        [
            request.FirstPriority,
            request.SecondPriority,
            request.ThirdPriority,
            request.ThirdPriority,
            request.FifthPriority,
            request.SixthPriority,
            request.SeventhPriority,
            request.EighthPriority,
        ];

        List<RotationType?> Alternatives =
        [
            request.FirstAlternative,
            request.SecondAlternative,
            request.ThirdAlternative,
        ];

        List<RotationType?> Avoids = [request.FirstAvoid, request.SecondAvoid, request.ThirdAvoid];

        List<RotationTypeResponse> PrioritiesResponse =
                [.. Priorities
                    .Where((priority) => priority != null)
                    .Cast<RotationType>()
                    .Select(RotationTypeConverter.CreateRotationTypeResponse)];

        List<RotationTypeResponse> AlternativesResponse =
                [.. Alternatives
                    .Where((alternative) => alternative != null)
                    .Cast<RotationType>()
                    .Select(RotationTypeConverter.CreateRotationTypeResponse)];

        List<RotationTypeResponse> AvoidsResponse =
                [.. Avoids
                    .Where((avoid) => avoid != null)
                    .Cast<RotationType>()
                    .Select(RotationTypeConverter.CreateRotationTypeResponse)];

        return new RotationPrefResponse
        {
            RotationPrefRequestId = request.RotationPrefRequestId,
            Priorities = PrioritiesResponse,
            Alternatives = AlternativesResponse,
            Avoids = AvoidsResponse,
            AdditionalNotes = request.AdditionalNotes
        };
    }
}