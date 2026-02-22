namespace MedicalDemo.Enums;

[Flags]
public enum PgyYearFlags
{
    None = 0,
    Pgy1 = 1 << 0, // 1
    Pgy2 = 1 << 1, // 2
    Pgy3 = 1 << 2, // 4
    Pgy4 = 1 << 3, // 8
}