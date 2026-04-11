namespace MedicalDemo.Enums;

[Flags]
public enum PartOfDay
{
    Morning = 1,
    Afternoon = 2,
    AllDay = Morning | Afternoon,
}

public static class PartOfDayExtensions
{
    extension(PartOfDay partOfDay)
    {
        public string? DbChar => partOfDay switch
        {
            PartOfDay.Morning => "A",
            PartOfDay.Afternoon => "P",
            PartOfDay.Morning | PartOfDay.Afternoon => null,
            _ => throw new ArgumentOutOfRangeException(nameof(partOfDay), partOfDay, null)
        };

        public static PartOfDay FromDbChar(string? dbChar) => dbChar switch
        {
            "A" => PartOfDay.Morning,
            "P" => PartOfDay.Afternoon,
            null => PartOfDay.Morning | PartOfDay.Afternoon,
            _ => throw new ArgumentOutOfRangeException(nameof(dbChar), dbChar, null)
        };
    }
}