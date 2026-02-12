namespace MedicalDemo.Enums;

[Flags]
public enum PgyYearFlags
{
    NONE = 0,
    PGY1 = 1 << 0, // 1
    PGY2 = 1 << 1, // 2
    PGY3 = 1 << 2, // 4
    PGY4 = 1 << 3, // 8
}