using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Converters;

public class RotationTypeConverter
{
    public static RotationTypeResponse CreateRotationTypeResponse(RotationType rotationType)
    {
        List<int> pgyYears = [];

        foreach (int i in new List<int> { 1, 2, 3, 4 })
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
}