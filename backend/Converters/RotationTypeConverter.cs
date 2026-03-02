using MedicalDemo.Enums;
using MedicalDemo.Extensions;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Converters;

public class RotationTypeConverter
{
    private readonly Dictionary<string, Pgy4RotationTypeEnum> stringToRotationTypeEnumDictionary =
    [];
    private bool isRotationTypesDictionaryPopulated = false;

    public RotationTypeResponse CreateRotationTypeResponse(RotationType rotationType)
    {
        List<int> pgyYears = [];

        for (int i = 1; i < 5; i++)
        {
            PgyYearFlags pgyYearFlags = (PgyYearFlags)(1 << (i - 1));
            if ((rotationType.PgyYearFlags & pgyYearFlags) != 0)
            {
                pgyYears.Add(i);
            }
        }

        return new RotationTypeResponse
        {
            RotationTypeId = rotationType.RotationTypeId,
            RotationName = rotationType.RotationName,
            DoesLongCall = rotationType.DoesLongCall,
            DoesShortCall = rotationType.DoesShortCall,
            DoesTrainingLongCall = rotationType.DoesTrainingLongCall,
            DoesTrainingShortCall = rotationType.DoesTrainingShortCall,
            IsChiefRotation = rotationType.IsChiefRotation,
            PgyYears = pgyYears,
        };
    }

    public Pgy4RotationTypeEnum ConvertRotationTypeModelToEnum(RotationType rotationTypeModel)
    {
        // Populate rotation type enum dictionary if needed
        if (!isRotationTypesDictionaryPopulated)
        {
            foreach (Pgy4RotationTypeEnum rotationTypeEnum in Enum.GetValues<Pgy4RotationTypeEnum>())
            {
                // Find corresponding typeEnum by comparing it to display name, add it dictionary
                string displayName = rotationTypeEnum.GetDisplayName();
                stringToRotationTypeEnumDictionary.Add(displayName, rotationTypeEnum);
            }

            isRotationTypesDictionaryPopulated = true;
        }

        // Try to get typeEnum from dictionary
        if (
            stringToRotationTypeEnumDictionary.TryGetValue(
                rotationTypeModel.RotationName,
                out Pgy4RotationTypeEnum value
            )
        )
        {
            return value;
        }

        // Throw error if it cannot find the corresponding rotation type enum
        throw new ArgumentException(
            "ERROR: Failed to convert RotationType to AlgorithmRotationType because it cannot find the corresponding enum!"
        );
    }
}