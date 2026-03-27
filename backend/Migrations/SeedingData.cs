using MedicalDemo.Enums;
using MedicalDemo.Extensions;

namespace MedicalDemo.Migrations;

[Obsolete("Do not use seeding data")]
public class SeedingData
{
    // This data is specifically only for seeding. It should never be used outside of Migrations
    public static readonly byte[] InpatientPsy = [43, 16, 19, 7, 193, 3, 98, 74, 191, 216, 76, 4, 54, 154, 199, 196];
    public static readonly byte[] ImInpatient = [127, 107, 64, 16, 183, 28, 13, 77, 177, 78, 121, 131, 164, 140, 28, 232];
    public static readonly byte[] NightFloat = [198, 128, 90, 38, 155, 98, 143, 70, 131, 234, 116, 101, 192, 246, 51, 195];
    public static readonly byte[] PhpAndIop = [165, 18, 107, 68, 24, 126, 253, 72, 138, 187, 32, 211, 154, 90, 163, 83];
    public static readonly byte[] Iop = [191, 177, 71, 69, 124, 87, 217, 74, 130, 70, 127, 8, 94, 217, 87, 249];
    public static readonly byte[] ImOutpatient = [88, 181, 70, 110, 90, 85, 4, 68, 129, 85, 19, 29, 197, 159, 164, 48];
    public static readonly byte[] Addiction = [215, 39, 22, 119, 159, 186, 73, 69, 137, 138, 66, 103, 253, 20, 228, 58];
    public static readonly byte[] Hpc = [65, 201, 67, 126, 192, 225, 88, 76, 143, 65, 83, 94, 26, 26, 130, 219];
    public static readonly byte[] Clc = [41, 63, 243, 128, 169, 207, 66, 68, 180, 88, 117, 40, 90, 217, 147, 196];
    public static readonly byte[] CommunityPsy = [110, 22, 50, 137, 36, 220, 60, 79, 136, 215, 63, 185, 238, 126, 22, 26];
    public static readonly byte[] Cap = [209, 143, 230, 137, 204, 40, 127, 77, 176, 253, 32, 254, 216, 57, 98, 49];
    public static readonly byte[] EmergencyMed = [81, 121, 28, 144, 20, 87, 143, 72, 151, 49, 99, 107, 225, 251, 106, 236];
    public static readonly byte[] Sum = [129, 242, 99, 144, 25, 121, 55, 73, 180, 107, 234, 4, 20, 112, 24, 202];
    public static readonly byte[] Va = [217, 68, 77, 152, 241, 47, 25, 73, 168, 149, 204, 134, 90, 158, 72, 114];
    public static readonly byte[] PsyConsults = [215, 87, 101, 160, 153, 16, 236, 66, 156, 238, 237, 62, 236, 98, 19, 171];
    public static readonly byte[] Neurology = [219, 221, 77, 165, 69, 2, 130, 69, 135, 54, 87, 227, 13, 138, 12, 23];
    public static readonly byte[] Float = [146, 142, 124, 165, 250, 119, 15, 65, 190, 84, 145, 38, 136, 28, 53, 20];
    public static readonly byte[] Nfetc = [50, 124, 227, 176, 25, 121, 82, 77, 191, 32, 242, 114, 201, 138, 78, 131];
    public static readonly byte[] Forensic = [44, 1, 68, 181, 224, 204, 217, 73, 178, 214, 31, 23, 6, 141, 183, 210];
    public static readonly byte[] Chief = [78, 40, 245, 224, 213, 0, 234, 76, 186, 46, 163, 113, 141, 91, 112, 196];
    public static readonly byte[] Tms = [16, 45, 246, 231, 218, 87, 73, 71, 156, 16, 11, 78, 197, 56, 160, 71];
    public static readonly byte[] Unassigned = [163, 198, 184, 245, 23, 100, 68, 75, 189, 163, 202, 94, 123, 238, 8, 109];
    public static readonly byte[] Geriatric = [234, 143, 171, 248, 255, 175, 221, 70, 174, 199, 66, 61, 121, 28, 232, 209];

    // PGY-1 Rotation IDs
    public static readonly byte[] Pgy1Rotation1 = [0xA1, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01];
    public static readonly byte[] Pgy1Rotation2 = [0xA1, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02];
    public static readonly byte[] Pgy1Rotation3 = [0xA1, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03];
    public static readonly byte[] Pgy1Rotation4 = [0xA1, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04];
    public static readonly byte[] Pgy1Rotation5 = [0xA1, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05];
    public static readonly byte[] Pgy1Rotation6 = [0xA1, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06];
    public static readonly byte[] Pgy1Rotation7 = [0xA1, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07];
    public static readonly byte[] Pgy1Rotation8 = [0xA1, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08];

    // PGY-2 Rotation IDs
    public static readonly byte[] Pgy2Rotation1 = [0xB1, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01];
    public static readonly byte[] Pgy2Rotation2 = [0xB1, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02];
    public static readonly byte[] Pgy2Rotation3 = [0xB1, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03];
    public static readonly byte[] Pgy2Rotation4 = [0xB1, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04];
    public static readonly byte[] Pgy2Rotation5 = [0xB1, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05];
    public static readonly byte[] Pgy2Rotation6 = [0xB1, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06];
    public static readonly byte[] Pgy2Rotation7 = [0xB1, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07];
    public static readonly byte[] Pgy2Rotation8 = [0xB1, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08];

    public static readonly object[,] Pgy1Rotations = new object[,]
    {
        // Row 1: Consult, Inpt Psy, Inpt Psy, Inpt Psy, Inpt Psy, ER-P, IMOP, Neuro, EM, IMOP, IMIP, Neuro
        { Pgy1Rotation1, 2025, (int) MonthOfYear.July,      null!, null!, MonthOfYear.July.GetDisplayName(),      1, PsyConsults  },
        { Pgy1Rotation1, 2025, (int) MonthOfYear.August,    null!, null!, MonthOfYear.August.GetDisplayName(),    1, InpatientPsy },
        { Pgy1Rotation1, 2025, (int) MonthOfYear.September, null!, null!, MonthOfYear.September.GetDisplayName(), 1, InpatientPsy },
        { Pgy1Rotation1, 2025, (int) MonthOfYear.October,   null!, null!, MonthOfYear.October.GetDisplayName(),   1, InpatientPsy },
        { Pgy1Rotation1, 2025, (int) MonthOfYear.November,  null!, null!, MonthOfYear.November.GetDisplayName(),  1, InpatientPsy },
        { Pgy1Rotation1, 2025, (int) MonthOfYear.December,  null!, null!, MonthOfYear.December.GetDisplayName(),  1, NightFloat   },
        { Pgy1Rotation1, 2025, (int) MonthOfYear.January,   null!, null!, MonthOfYear.January.GetDisplayName(),   1, ImOutpatient },
        { Pgy1Rotation1, 2025, (int) MonthOfYear.February,  null!, null!, MonthOfYear.February.GetDisplayName(),  1, Neurology    },
        { Pgy1Rotation1, 2025, (int) MonthOfYear.March,     null!, null!, MonthOfYear.March.GetDisplayName(),     1, EmergencyMed },
        { Pgy1Rotation1, 2025, (int) MonthOfYear.April,     null!, null!, MonthOfYear.April.GetDisplayName(),     1, ImOutpatient },
        { Pgy1Rotation1, 2025, (int) MonthOfYear.May,       null!, null!, MonthOfYear.May.GetDisplayName(),       1, ImInpatient  },
        { Pgy1Rotation1, 2025, (int) MonthOfYear.June,      null!, null!, MonthOfYear.June.GetDisplayName(),      1, Neurology    },

        // Row 2: Inpt Psy, Inpt Psy, Inpt Psy, Consult, ER-P, Inpt Psy, EM, IMOP, Neuro, IMIP, IMOP, Neuro
        { Pgy1Rotation2, 2025, (int) MonthOfYear.July,      null!, null!, MonthOfYear.July.GetDisplayName(),      1, InpatientPsy },
        { Pgy1Rotation2, 2025, (int) MonthOfYear.August,    null!, null!, MonthOfYear.August.GetDisplayName(),    1, InpatientPsy },
        { Pgy1Rotation2, 2025, (int) MonthOfYear.September, null!, null!, MonthOfYear.September.GetDisplayName(), 1, InpatientPsy },
        { Pgy1Rotation2, 2025, (int) MonthOfYear.October,   null!, null!, MonthOfYear.October.GetDisplayName(),   1, PsyConsults  },
        { Pgy1Rotation2, 2025, (int) MonthOfYear.November,  null!, null!, MonthOfYear.November.GetDisplayName(),  1, NightFloat   },
        { Pgy1Rotation2, 2025, (int) MonthOfYear.December,  null!, null!, MonthOfYear.December.GetDisplayName(),  1, InpatientPsy },
        { Pgy1Rotation2, 2025, (int) MonthOfYear.January,   null!, null!, MonthOfYear.January.GetDisplayName(),   1, EmergencyMed },
        { Pgy1Rotation2, 2025, (int) MonthOfYear.February,  null!, null!, MonthOfYear.February.GetDisplayName(),  1, ImOutpatient },
        { Pgy1Rotation2, 2025, (int) MonthOfYear.March,     null!, null!, MonthOfYear.March.GetDisplayName(),     1, Neurology    },
        { Pgy1Rotation2, 2025, (int) MonthOfYear.April,     null!, null!, MonthOfYear.April.GetDisplayName(),     1, ImInpatient  },
        { Pgy1Rotation2, 2025, (int) MonthOfYear.May,       null!, null!, MonthOfYear.May.GetDisplayName(),       1, ImOutpatient },
        { Pgy1Rotation2, 2025, (int) MonthOfYear.June,      null!, null!, MonthOfYear.June.GetDisplayName(),      1, Neurology    },

        // Row 3: Inpt Psy, Inpt Psy, Consult, ER-P, Inpt Psy, Inpt Psy, Neuro, IMIP, IMOP, Neuro, EM, IMOP
        { Pgy1Rotation3, 2025, (int) MonthOfYear.July,      null!, null!, MonthOfYear.July.GetDisplayName(),      1, InpatientPsy },
        { Pgy1Rotation3, 2025, (int) MonthOfYear.August,    null!, null!, MonthOfYear.August.GetDisplayName(),    1, InpatientPsy },
        { Pgy1Rotation3, 2025, (int) MonthOfYear.September, null!, null!, MonthOfYear.September.GetDisplayName(), 1, PsyConsults  },
        { Pgy1Rotation3, 2025, (int) MonthOfYear.October,   null!, null!, MonthOfYear.October.GetDisplayName(),   1, NightFloat   },
        { Pgy1Rotation3, 2025, (int) MonthOfYear.November,  null!, null!, MonthOfYear.November.GetDisplayName(),  1, InpatientPsy },
        { Pgy1Rotation3, 2025, (int) MonthOfYear.December,  null!, null!, MonthOfYear.December.GetDisplayName(),  1, InpatientPsy },
        { Pgy1Rotation3, 2025, (int) MonthOfYear.January,   null!, null!, MonthOfYear.January.GetDisplayName(),   1, Neurology    },
        { Pgy1Rotation3, 2025, (int) MonthOfYear.February,  null!, null!, MonthOfYear.February.GetDisplayName(),  1, ImInpatient  },
        { Pgy1Rotation3, 2025, (int) MonthOfYear.March,     null!, null!, MonthOfYear.March.GetDisplayName(),     1, ImOutpatient },
        { Pgy1Rotation3, 2025, (int) MonthOfYear.April,     null!, null!, MonthOfYear.April.GetDisplayName(),     1, Neurology    },
        { Pgy1Rotation3, 2025, (int) MonthOfYear.May,       null!, null!, MonthOfYear.May.GetDisplayName(),       1, EmergencyMed },
        { Pgy1Rotation3, 2025, (int) MonthOfYear.June,      null!, null!, MonthOfYear.June.GetDisplayName(),      1, ImOutpatient },

        // Row 4: Inpt Psy, Consult, ER-P, Inpt Psy, Inpt Psy, Inpt Psy, Neuro, IMOP, IMIP, IMOP, Neuro, EM
        { Pgy1Rotation4, 2025, (int) MonthOfYear.July,      null!, null!, MonthOfYear.July.GetDisplayName(),      1, InpatientPsy },
        { Pgy1Rotation4, 2025, (int) MonthOfYear.August,    null!, null!, MonthOfYear.August.GetDisplayName(),    1, PsyConsults  },
        { Pgy1Rotation4, 2025, (int) MonthOfYear.September, null!, null!, MonthOfYear.September.GetDisplayName(), 1, NightFloat   },
        { Pgy1Rotation4, 2025, (int) MonthOfYear.October,   null!, null!, MonthOfYear.October.GetDisplayName(),   1, InpatientPsy },
        { Pgy1Rotation4, 2025, (int) MonthOfYear.November,  null!, null!, MonthOfYear.November.GetDisplayName(),  1, InpatientPsy },
        { Pgy1Rotation4, 2025, (int) MonthOfYear.December,  null!, null!, MonthOfYear.December.GetDisplayName(),  1, InpatientPsy },
        { Pgy1Rotation4, 2025, (int) MonthOfYear.January,   null!, null!, MonthOfYear.January.GetDisplayName(),   1, Neurology    },
        { Pgy1Rotation4, 2025, (int) MonthOfYear.February,  null!, null!, MonthOfYear.February.GetDisplayName(),  1, ImOutpatient },
        { Pgy1Rotation4, 2025, (int) MonthOfYear.March,     null!, null!, MonthOfYear.March.GetDisplayName(),     1, ImInpatient  },
        { Pgy1Rotation4, 2025, (int) MonthOfYear.April,     null!, null!, MonthOfYear.April.GetDisplayName(),     1, ImOutpatient },
        { Pgy1Rotation4, 2025, (int) MonthOfYear.May,       null!, null!, MonthOfYear.May.GetDisplayName(),       1, Neurology    },
        { Pgy1Rotation4, 2025, (int) MonthOfYear.June,      null!, null!, MonthOfYear.June.GetDisplayName(),      1, EmergencyMed },

        // Row 5: Neuro, EM, IMOP, IMIP, Neuro, IMOP, Consult, Inpt Psy, ER-P, Inpt Psy, Inpt Psy, Inpt Psy
        { Pgy1Rotation5, 2025, (int) MonthOfYear.July,      null!, null!, MonthOfYear.July.GetDisplayName(),      1, Neurology    },
        { Pgy1Rotation5, 2025, (int) MonthOfYear.August,    null!, null!, MonthOfYear.August.GetDisplayName(),    1, EmergencyMed },
        { Pgy1Rotation5, 2025, (int) MonthOfYear.September, null!, null!, MonthOfYear.September.GetDisplayName(), 1, ImOutpatient },
        { Pgy1Rotation5, 2025, (int) MonthOfYear.October,   null!, null!, MonthOfYear.October.GetDisplayName(),   1, ImInpatient  },
        { Pgy1Rotation5, 2025, (int) MonthOfYear.November,  null!, null!, MonthOfYear.November.GetDisplayName(),  1, Neurology    },
        { Pgy1Rotation5, 2025, (int) MonthOfYear.December,  null!, null!, MonthOfYear.December.GetDisplayName(),  1, ImOutpatient },
        { Pgy1Rotation5, 2025, (int) MonthOfYear.January,   null!, null!, MonthOfYear.January.GetDisplayName(),   1, PsyConsults  },
        { Pgy1Rotation5, 2025, (int) MonthOfYear.February,  null!, null!, MonthOfYear.February.GetDisplayName(),  1, InpatientPsy },
        { Pgy1Rotation5, 2025, (int) MonthOfYear.March,     null!, null!, MonthOfYear.March.GetDisplayName(),     1, NightFloat   },
        { Pgy1Rotation5, 2025, (int) MonthOfYear.April,     null!, null!, MonthOfYear.April.GetDisplayName(),     1, InpatientPsy },
        { Pgy1Rotation5, 2025, (int) MonthOfYear.May,       null!, null!, MonthOfYear.May.GetDisplayName(),       1, InpatientPsy },
        { Pgy1Rotation5, 2025, (int) MonthOfYear.June,      null!, null!, MonthOfYear.June.GetDisplayName(),      1, InpatientPsy },

        // Row 6: Neuro, IMOP, EM, IMOP, IMIP, Neuro, Inpt Psy, Consult, Inpt Psy, ER-P, Inpt Psy, Inpt Psy
        { Pgy1Rotation6, 2025, (int) MonthOfYear.July,      null!, null!, MonthOfYear.July.GetDisplayName(),      1, Neurology    },
        { Pgy1Rotation6, 2025, (int) MonthOfYear.August,    null!, null!, MonthOfYear.August.GetDisplayName(),    1, ImOutpatient },
        { Pgy1Rotation6, 2025, (int) MonthOfYear.September, null!, null!, MonthOfYear.September.GetDisplayName(), 1, EmergencyMed },
        { Pgy1Rotation6, 2025, (int) MonthOfYear.October,   null!, null!, MonthOfYear.October.GetDisplayName(),   1, ImOutpatient },
        { Pgy1Rotation6, 2025, (int) MonthOfYear.November,  null!, null!, MonthOfYear.November.GetDisplayName(),  1, ImInpatient  },
        { Pgy1Rotation6, 2025, (int) MonthOfYear.December,  null!, null!, MonthOfYear.December.GetDisplayName(),  1, Neurology    },
        { Pgy1Rotation6, 2025, (int) MonthOfYear.January,   null!, null!, MonthOfYear.January.GetDisplayName(),   1, InpatientPsy },
        { Pgy1Rotation6, 2025, (int) MonthOfYear.February,  null!, null!, MonthOfYear.February.GetDisplayName(),  1, PsyConsults  },
        { Pgy1Rotation6, 2025, (int) MonthOfYear.March,     null!, null!, MonthOfYear.March.GetDisplayName(),     1, InpatientPsy },
        { Pgy1Rotation6, 2025, (int) MonthOfYear.April,     null!, null!, MonthOfYear.April.GetDisplayName(),     1, NightFloat   },
        { Pgy1Rotation6, 2025, (int) MonthOfYear.May,       null!, null!, MonthOfYear.May.GetDisplayName(),       1, InpatientPsy },
        { Pgy1Rotation6, 2025, (int) MonthOfYear.June,      null!, null!, MonthOfYear.June.GetDisplayName(),      1, InpatientPsy },

        // Row 7: IMOP, IMIP, Neuro, Neuro, EM, IMOP, Inpt Psy, Inpt Psy, Consult, Inpt Psy, ER-P, Inpt Psy
        { Pgy1Rotation7, 2025, (int) MonthOfYear.July,      null!, null!, MonthOfYear.July.GetDisplayName(),      1, ImOutpatient },
        { Pgy1Rotation7, 2025, (int) MonthOfYear.August,    null!, null!, MonthOfYear.August.GetDisplayName(),    1, ImInpatient  },
        { Pgy1Rotation7, 2025, (int) MonthOfYear.September, null!, null!, MonthOfYear.September.GetDisplayName(), 1, Neurology    },
        { Pgy1Rotation7, 2025, (int) MonthOfYear.October,   null!, null!, MonthOfYear.October.GetDisplayName(),   1, Neurology    },
        { Pgy1Rotation7, 2025, (int) MonthOfYear.November,  null!, null!, MonthOfYear.November.GetDisplayName(),  1, EmergencyMed },
        { Pgy1Rotation7, 2025, (int) MonthOfYear.December,  null!, null!, MonthOfYear.December.GetDisplayName(),  1, ImOutpatient },
        { Pgy1Rotation7, 2025, (int) MonthOfYear.January,   null!, null!, MonthOfYear.January.GetDisplayName(),   1, InpatientPsy },
        { Pgy1Rotation7, 2025, (int) MonthOfYear.February,  null!, null!, MonthOfYear.February.GetDisplayName(),  1, InpatientPsy },
        { Pgy1Rotation7, 2025, (int) MonthOfYear.March,     null!, null!, MonthOfYear.March.GetDisplayName(),     1, PsyConsults  },
        { Pgy1Rotation7, 2025, (int) MonthOfYear.April,     null!, null!, MonthOfYear.April.GetDisplayName(),     1, InpatientPsy },
        { Pgy1Rotation7, 2025, (int) MonthOfYear.May,       null!, null!, MonthOfYear.May.GetDisplayName(),       1, NightFloat   },
        { Pgy1Rotation7, 2025, (int) MonthOfYear.June,      null!, null!, MonthOfYear.June.GetDisplayName(),      1, InpatientPsy },

        // Row 8: EM, Neuro, IMIP, IMOP, IMOP, Neuro, Inpt Psy, Inpt Psy, Inpt Psy, Consult, Inpt Psy, ER-P
        { Pgy1Rotation8, 2025, (int) MonthOfYear.July,      null!, null!, MonthOfYear.July.GetDisplayName(),      1, EmergencyMed },
        { Pgy1Rotation8, 2025, (int) MonthOfYear.August,    null!, null!, MonthOfYear.August.GetDisplayName(),    1, Neurology    },
        { Pgy1Rotation8, 2025, (int) MonthOfYear.September, null!, null!, MonthOfYear.September.GetDisplayName(), 1, ImInpatient  },
        { Pgy1Rotation8, 2025, (int) MonthOfYear.October,   null!, null!, MonthOfYear.October.GetDisplayName(),   1, ImOutpatient },
        { Pgy1Rotation8, 2025, (int) MonthOfYear.November,  null!, null!, MonthOfYear.November.GetDisplayName(),  1, ImOutpatient },
        { Pgy1Rotation8, 2025, (int) MonthOfYear.December,  null!, null!, MonthOfYear.December.GetDisplayName(),  1, Neurology    },
        { Pgy1Rotation8, 2025, (int) MonthOfYear.January,   null!, null!, MonthOfYear.January.GetDisplayName(),   1, InpatientPsy },
        { Pgy1Rotation8, 2025, (int) MonthOfYear.February,  null!, null!, MonthOfYear.February.GetDisplayName(),  1, InpatientPsy },
        { Pgy1Rotation8, 2025, (int) MonthOfYear.March,     null!, null!, MonthOfYear.March.GetDisplayName(),     1, InpatientPsy },
        { Pgy1Rotation8, 2025, (int) MonthOfYear.April,     null!, null!, MonthOfYear.April.GetDisplayName(),     1, PsyConsults  },
        { Pgy1Rotation8, 2025, (int) MonthOfYear.May,       null!, null!, MonthOfYear.May.GetDisplayName(),       1, InpatientPsy },
        { Pgy1Rotation8, 2025, (int) MonthOfYear.June,      null!, null!, MonthOfYear.June.GetDisplayName(),      1, NightFloat   },
    };

    public static readonly object[,] Pgy2Rotations = new object[,]
    {
        // Row 1: Child, Child, Comm, Forensic, Float, Addiction, Inpt Psy, ER P/CL, PHP/IOP, Consult, Geri, Inpt Psy
        { Pgy2Rotation1, 2025, (int) MonthOfYear.July,      null!, null!, MonthOfYear.July.GetDisplayName(),      2, Cap          },
        { Pgy2Rotation1, 2025, (int) MonthOfYear.August,    null!, null!, MonthOfYear.August.GetDisplayName(),    2, Cap          },
        { Pgy2Rotation1, 2025, (int) MonthOfYear.September, null!, null!, MonthOfYear.September.GetDisplayName(), 2, CommunityPsy },
        { Pgy2Rotation1, 2025, (int) MonthOfYear.October,   null!, null!, MonthOfYear.October.GetDisplayName(),   2, Forensic     },
        { Pgy2Rotation1, 2025, (int) MonthOfYear.November,  null!, null!, MonthOfYear.November.GetDisplayName(),  2, Float        },
        { Pgy2Rotation1, 2025, (int) MonthOfYear.December,  null!, null!, MonthOfYear.December.GetDisplayName(),  2, Addiction    },
        { Pgy2Rotation1, 2025, (int) MonthOfYear.January,   null!, null!, MonthOfYear.January.GetDisplayName(),   2, InpatientPsy },
        { Pgy2Rotation1, 2025, (int) MonthOfYear.February,  null!, null!, MonthOfYear.February.GetDisplayName(),  2, NightFloat   },
        { Pgy2Rotation1, 2025, (int) MonthOfYear.March,     null!, null!, MonthOfYear.March.GetDisplayName(),     2, PhpAndIop    },
        { Pgy2Rotation1, 2025, (int) MonthOfYear.April,     null!, null!, MonthOfYear.April.GetDisplayName(),     2, PsyConsults  },
        { Pgy2Rotation1, 2025, (int) MonthOfYear.May,       null!, null!, MonthOfYear.May.GetDisplayName(),       2, Geriatric    },
        { Pgy2Rotation1, 2025, (int) MonthOfYear.June,      null!, null!, MonthOfYear.June.GetDisplayName(),      2, InpatientPsy },

        // Row 2: Addiction, Child, Child, Float, Forensic, Comm, CL/ER P, Inpt Psy, Consult, PHP/IOP, Inpt Psy, Geri
        { Pgy2Rotation2, 2025, (int) MonthOfYear.July,      null!, null!, MonthOfYear.July.GetDisplayName(),      2, Addiction    },
        { Pgy2Rotation2, 2025, (int) MonthOfYear.August,    null!, null!, MonthOfYear.August.GetDisplayName(),    2, Cap          },
        { Pgy2Rotation2, 2025, (int) MonthOfYear.September, null!, null!, MonthOfYear.September.GetDisplayName(), 2, Cap          },
        { Pgy2Rotation2, 2025, (int) MonthOfYear.October,   null!, null!, MonthOfYear.October.GetDisplayName(),   2, Float        },
        { Pgy2Rotation2, 2025, (int) MonthOfYear.November,  null!, null!, MonthOfYear.November.GetDisplayName(),  2, Forensic     },
        { Pgy2Rotation2, 2025, (int) MonthOfYear.December,  null!, null!, MonthOfYear.December.GetDisplayName(),  2, CommunityPsy },
        { Pgy2Rotation2, 2025, (int) MonthOfYear.January,   null!, null!, MonthOfYear.January.GetDisplayName(),   2, NightFloat   },
        { Pgy2Rotation2, 2025, (int) MonthOfYear.February,  null!, null!, MonthOfYear.February.GetDisplayName(),  2, InpatientPsy },
        { Pgy2Rotation2, 2025, (int) MonthOfYear.March,     null!, null!, MonthOfYear.March.GetDisplayName(),     2, PsyConsults  },
        { Pgy2Rotation2, 2025, (int) MonthOfYear.April,     null!, null!, MonthOfYear.April.GetDisplayName(),     2, PhpAndIop    },
        { Pgy2Rotation2, 2025, (int) MonthOfYear.May,       null!, null!, MonthOfYear.May.GetDisplayName(),       2, InpatientPsy },
        { Pgy2Rotation2, 2025, (int) MonthOfYear.June,      null!, null!, MonthOfYear.June.GetDisplayName(),      2, Geriatric    },

        // Row 3: Forensic, Addiction, Child, Child, Comm, Float, Geri, CL/ER P, Inpt Psy, Inpt Psy, PHP/IOP, Consult
        { Pgy2Rotation3, 2025, (int) MonthOfYear.July,      null!, null!, MonthOfYear.July.GetDisplayName(),      2, Forensic     },
        { Pgy2Rotation3, 2025, (int) MonthOfYear.August,    null!, null!, MonthOfYear.August.GetDisplayName(),    2, Addiction    },
        { Pgy2Rotation3, 2025, (int) MonthOfYear.September, null!, null!, MonthOfYear.September.GetDisplayName(), 2, Cap          },
        { Pgy2Rotation3, 2025, (int) MonthOfYear.October,   null!, null!, MonthOfYear.October.GetDisplayName(),   2, Cap          },
        { Pgy2Rotation3, 2025, (int) MonthOfYear.November,  null!, null!, MonthOfYear.November.GetDisplayName(),  2, CommunityPsy },
        { Pgy2Rotation3, 2025, (int) MonthOfYear.December,  null!, null!, MonthOfYear.December.GetDisplayName(),  2, Float        },
        { Pgy2Rotation3, 2025, (int) MonthOfYear.January,   null!, null!, MonthOfYear.January.GetDisplayName(),   2, Geriatric    },
        { Pgy2Rotation3, 2025, (int) MonthOfYear.February,  null!, null!, MonthOfYear.February.GetDisplayName(),  2, NightFloat   },
        { Pgy2Rotation3, 2025, (int) MonthOfYear.March,     null!, null!, MonthOfYear.March.GetDisplayName(),     2, InpatientPsy },
        { Pgy2Rotation3, 2025, (int) MonthOfYear.April,     null!, null!, MonthOfYear.April.GetDisplayName(),     2, InpatientPsy },
        { Pgy2Rotation3, 2025, (int) MonthOfYear.May,       null!, null!, MonthOfYear.May.GetDisplayName(),       2, PhpAndIop    },
        { Pgy2Rotation3, 2025, (int) MonthOfYear.June,      null!, null!, MonthOfYear.June.GetDisplayName(),      2, PsyConsults  },

        // Row 4: Comm, Forensic, Float, Addiction, Child, Child, ER P/CL, Geri, Inpt Psy, Inpt Psy, Consult, PHP/IOP
        { Pgy2Rotation4, 2025, (int) MonthOfYear.July,      null!, null!, MonthOfYear.July.GetDisplayName(),      2, CommunityPsy },
        { Pgy2Rotation4, 2025, (int) MonthOfYear.August,    null!, null!, MonthOfYear.August.GetDisplayName(),    2, Forensic     },
        { Pgy2Rotation4, 2025, (int) MonthOfYear.September, null!, null!, MonthOfYear.September.GetDisplayName(), 2, Float        },
        { Pgy2Rotation4, 2025, (int) MonthOfYear.October,   null!, null!, MonthOfYear.October.GetDisplayName(),   2, Addiction    },
        { Pgy2Rotation4, 2025, (int) MonthOfYear.November,  null!, null!, MonthOfYear.November.GetDisplayName(),  2, Cap          },
        { Pgy2Rotation4, 2025, (int) MonthOfYear.December,  null!, null!, MonthOfYear.December.GetDisplayName(),  2, Cap          },
        { Pgy2Rotation4, 2025, (int) MonthOfYear.January,   null!, null!, MonthOfYear.January.GetDisplayName(),   2, NightFloat   },
        { Pgy2Rotation4, 2025, (int) MonthOfYear.February,  null!, null!, MonthOfYear.February.GetDisplayName(),  2, Geriatric    },
        { Pgy2Rotation4, 2025, (int) MonthOfYear.March,     null!, null!, MonthOfYear.March.GetDisplayName(),     2, InpatientPsy },
        { Pgy2Rotation4, 2025, (int) MonthOfYear.April,     null!, null!, MonthOfYear.April.GetDisplayName(),     2, InpatientPsy },
        { Pgy2Rotation4, 2025, (int) MonthOfYear.May,       null!, null!, MonthOfYear.May.GetDisplayName(),       2, PsyConsults  },
        { Pgy2Rotation4, 2025, (int) MonthOfYear.June,      null!, null!, MonthOfYear.June.GetDisplayName(),      2, PhpAndIop    },

        // Row 5: ER P/CL, Geri, PHP/IOP, Inpt Psy, Inpt Psy, Consult, Child, Child, Addiction, Float, Forensic, Comm
        { Pgy2Rotation5, 2025, (int) MonthOfYear.July,      null!, null!, MonthOfYear.July.GetDisplayName(),      2, NightFloat   },
        { Pgy2Rotation5, 2025, (int) MonthOfYear.August,    null!, null!, MonthOfYear.August.GetDisplayName(),    2, Geriatric    },
        { Pgy2Rotation5, 2025, (int) MonthOfYear.September, null!, null!, MonthOfYear.September.GetDisplayName(), 2, PhpAndIop    },
        { Pgy2Rotation5, 2025, (int) MonthOfYear.October,   null!, null!, MonthOfYear.October.GetDisplayName(),   2, InpatientPsy },
        { Pgy2Rotation5, 2025, (int) MonthOfYear.November,  null!, null!, MonthOfYear.November.GetDisplayName(),  2, InpatientPsy },
        { Pgy2Rotation5, 2025, (int) MonthOfYear.December,  null!, null!, MonthOfYear.December.GetDisplayName(),  2, PsyConsults  },
        { Pgy2Rotation5, 2025, (int) MonthOfYear.January,   null!, null!, MonthOfYear.January.GetDisplayName(),   2, Cap          },
        { Pgy2Rotation5, 2025, (int) MonthOfYear.February,  null!, null!, MonthOfYear.February.GetDisplayName(),  2, Cap          },
        { Pgy2Rotation5, 2025, (int) MonthOfYear.March,     null!, null!, MonthOfYear.March.GetDisplayName(),     2, Addiction    },
        { Pgy2Rotation5, 2025, (int) MonthOfYear.April,     null!, null!, MonthOfYear.April.GetDisplayName(),     2, Float        },
        { Pgy2Rotation5, 2025, (int) MonthOfYear.May,       null!, null!, MonthOfYear.May.GetDisplayName(),       2, Forensic     },
        { Pgy2Rotation5, 2025, (int) MonthOfYear.June,      null!, null!, MonthOfYear.June.GetDisplayName(),      2, CommunityPsy },

        // Row 6: CL/ER P, Inpt Psy, Inpt Psy, Consult, PHP/IOP, Geri, Forensic, Comm, Float, Addiction, Child, Child
        { Pgy2Rotation6, 2025, (int) MonthOfYear.July,      null!, null!, MonthOfYear.July.GetDisplayName(),      2, NightFloat   },
        { Pgy2Rotation6, 2025, (int) MonthOfYear.August,    null!, null!, MonthOfYear.August.GetDisplayName(),    2, InpatientPsy },
        { Pgy2Rotation6, 2025, (int) MonthOfYear.September, null!, null!, MonthOfYear.September.GetDisplayName(), 2, InpatientPsy },
        { Pgy2Rotation6, 2025, (int) MonthOfYear.October,   null!, null!, MonthOfYear.October.GetDisplayName(),   2, PsyConsults  },
        { Pgy2Rotation6, 2025, (int) MonthOfYear.November,  null!, null!, MonthOfYear.November.GetDisplayName(),  2, PhpAndIop    },
        { Pgy2Rotation6, 2025, (int) MonthOfYear.December,  null!, null!, MonthOfYear.December.GetDisplayName(),  2, Geriatric    },
        { Pgy2Rotation6, 2025, (int) MonthOfYear.January,   null!, null!, MonthOfYear.January.GetDisplayName(),   2, Forensic     },
        { Pgy2Rotation6, 2025, (int) MonthOfYear.February,  null!, null!, MonthOfYear.February.GetDisplayName(),  2, CommunityPsy },
        { Pgy2Rotation6, 2025, (int) MonthOfYear.March,     null!, null!, MonthOfYear.March.GetDisplayName(),     2, Float        },
        { Pgy2Rotation6, 2025, (int) MonthOfYear.April,     null!, null!, MonthOfYear.April.GetDisplayName(),     2, Addiction    },
        { Pgy2Rotation6, 2025, (int) MonthOfYear.May,       null!, null!, MonthOfYear.May.GetDisplayName(),       2, Cap          },
        { Pgy2Rotation6, 2025, (int) MonthOfYear.June,      null!, null!, MonthOfYear.June.GetDisplayName(),      2, Cap          },

        // Row 7: Inpt Psy, ER P/CL, Consult, PHP/IOP, Geri, Inpt Psy, Comm, Addiction, Forensic, Child, Child, Float
        { Pgy2Rotation7, 2025, (int) MonthOfYear.July,      null!, null!, MonthOfYear.July.GetDisplayName(),      2, InpatientPsy },
        { Pgy2Rotation7, 2025, (int) MonthOfYear.August,    null!, null!, MonthOfYear.August.GetDisplayName(),    2, NightFloat   },
        { Pgy2Rotation7, 2025, (int) MonthOfYear.September, null!, null!, MonthOfYear.September.GetDisplayName(), 2, PsyConsults  },
        { Pgy2Rotation7, 2025, (int) MonthOfYear.October,   null!, null!, MonthOfYear.October.GetDisplayName(),   2, PhpAndIop    },
        { Pgy2Rotation7, 2025, (int) MonthOfYear.November,  null!, null!, MonthOfYear.November.GetDisplayName(),  2, Geriatric    },
        { Pgy2Rotation7, 2025, (int) MonthOfYear.December,  null!, null!, MonthOfYear.December.GetDisplayName(),  2, InpatientPsy },
        { Pgy2Rotation7, 2025, (int) MonthOfYear.January,   null!, null!, MonthOfYear.January.GetDisplayName(),   2, CommunityPsy },
        { Pgy2Rotation7, 2025, (int) MonthOfYear.February,  null!, null!, MonthOfYear.February.GetDisplayName(),  2, Addiction    },
        { Pgy2Rotation7, 2025, (int) MonthOfYear.March,     null!, null!, MonthOfYear.March.GetDisplayName(),     2, Forensic     },
        { Pgy2Rotation7, 2025, (int) MonthOfYear.April,     null!, null!, MonthOfYear.April.GetDisplayName(),     2, Cap          },
        { Pgy2Rotation7, 2025, (int) MonthOfYear.May,       null!, null!, MonthOfYear.May.GetDisplayName(),       2, Cap          },
        { Pgy2Rotation7, 2025, (int) MonthOfYear.June,      null!, null!, MonthOfYear.June.GetDisplayName(),      2, Float        },

        // Row 8: Geri, CL/ER P, Inpt Psy, Inpt Psy, Consult, PHP/IOP, Addiction, Child, Child, Comm, Float, Forensic
        { Pgy2Rotation8, 2025, (int) MonthOfYear.July,      null!, null!, MonthOfYear.July.GetDisplayName(),      2, Geriatric    },
        { Pgy2Rotation8, 2025, (int) MonthOfYear.August,    null!, null!, MonthOfYear.August.GetDisplayName(),    2, NightFloat   },
        { Pgy2Rotation8, 2025, (int) MonthOfYear.September, null!, null!, MonthOfYear.September.GetDisplayName(), 2, InpatientPsy },
        { Pgy2Rotation8, 2025, (int) MonthOfYear.October,   null!, null!, MonthOfYear.October.GetDisplayName(),   2, InpatientPsy },
        { Pgy2Rotation8, 2025, (int) MonthOfYear.November,  null!, null!, MonthOfYear.November.GetDisplayName(),  2, PsyConsults  },
        { Pgy2Rotation8, 2025, (int) MonthOfYear.December,  null!, null!, MonthOfYear.December.GetDisplayName(),  2, PhpAndIop    },
        { Pgy2Rotation8, 2025, (int) MonthOfYear.January,   null!, null!, MonthOfYear.January.GetDisplayName(),   2, Addiction    },
        { Pgy2Rotation8, 2025, (int) MonthOfYear.February,  null!, null!, MonthOfYear.February.GetDisplayName(),  2, Cap          },
        { Pgy2Rotation8, 2025, (int) MonthOfYear.March,     null!, null!, MonthOfYear.March.GetDisplayName(),     2, Cap          },
        { Pgy2Rotation8, 2025, (int) MonthOfYear.April,     null!, null!, MonthOfYear.April.GetDisplayName(),     2, CommunityPsy },
        { Pgy2Rotation8, 2025, (int) MonthOfYear.May,       null!, null!, MonthOfYear.May.GetDisplayName(),       2, Float        },
        { Pgy2Rotation8, 2025, (int) MonthOfYear.June,      null!, null!, MonthOfYear.June.GetDisplayName(),      2, Forensic     },
    };
}