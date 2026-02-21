using MedicalDemo.Enums;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
using RotationScheduleGenerator.Algorithm;

namespace MedicalDemo.Converters;

public class RotationTypeConverter
{
    private static readonly Dictionary<
        string,
        PGY4RotationTypeEnum
    > stringToRotationTypeEnumDictionary = new()
    {
        { "Inpatient Psy", PGY4RotationTypeEnum.InpatientPsy },
        { "Forensic", PGY4RotationTypeEnum.Forensic },
        { "Community Psy", PGY4RotationTypeEnum.CommunityPsy },
        { "Addiction", PGY4RotationTypeEnum.Addiction },
        { "Psy Consults", PGY4RotationTypeEnum.PsyConsults },
        { "TMS", PGY4RotationTypeEnum.TMS },
        { "CLC", PGY4RotationTypeEnum.CLC },
        { "NFETC", PGY4RotationTypeEnum.NFETC },
        { "HPC", PGY4RotationTypeEnum.HPC },
        { "Chief", PGY4RotationTypeEnum.Chief },
        { "Sum", PGY4RotationTypeEnum.Sum },
        { "IOP", PGY4RotationTypeEnum.IOP },
        { "VA", PGY4RotationTypeEnum.VA },
    };

    private static readonly Dictionary<
        PGY4RotationTypeEnum,
        string
    > rotationTypeEnumToStringDictionary = new()
    {
        { PGY4RotationTypeEnum.InpatientPsy, "Inpatient Psy" },
        { PGY4RotationTypeEnum.Forensic, "Forensic" },
        { PGY4RotationTypeEnum.CommunityPsy, "Community Psy" },
        { PGY4RotationTypeEnum.Addiction, "Addiction" },
        { PGY4RotationTypeEnum.PsyConsults, "Psy Consults" },
        { PGY4RotationTypeEnum.TMS, "TMS" },
        { PGY4RotationTypeEnum.CLC, "CLC" },
        { PGY4RotationTypeEnum.NFETC, "NFETC" },
        { PGY4RotationTypeEnum.HPC, "HPC" },
        { PGY4RotationTypeEnum.Chief, "Chief" },
        { PGY4RotationTypeEnum.Sum, "Sum" },
        { PGY4RotationTypeEnum.IOP, "IOP" },
        { PGY4RotationTypeEnum.VA, "VA" },
    };

    public static RotationTypeResponse CreateRotationTypeResponse(RotationType rotationType)
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

    public static PGY4RotationTypeEnum ConvertRotationTypeModelToEnum(
        RotationType rotationTypeModel
    )
    {
        if (
            !stringToRotationTypeEnumDictionary.TryGetValue(
                rotationTypeModel.RotationName,
                out PGY4RotationTypeEnum value
            )
        )
        {
            throw new Exception(
                "ERROR: Failed to convert RotationType to AlgorithmRotationType because it cannot find the corresponding enum!"
            );
        }

        return value;
    }

    public static string ConvertRotationTypeEnumToName(PGY4RotationTypeEnum enumType)
    {
        if (!rotationTypeEnumToStringDictionary.TryGetValue(enumType, out string? value))
        {
            throw new Exception(
                "ERROR: Failed to convert RotationType to AlgorithmRotationType because it cannot find the corresponding enum!"
            );
        }

        return value;
    }
}
