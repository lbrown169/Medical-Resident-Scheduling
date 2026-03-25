using MedicalDemo.Enums;

namespace MedicalDemo.Extensions;

public static class PgyYearFlagsExtensions
{
    public static PgyYearFlags FromYear(int pgyYear) => pgyYear switch
    {
        1 => PgyYearFlags.Pgy1,
        2 => PgyYearFlags.Pgy2,
        3 => PgyYearFlags.Pgy3,
        4 => PgyYearFlags.Pgy4,
        _ => throw new ArgumentOutOfRangeException(nameof(pgyYear), $"Invalid PGY year: {pgyYear}")
    };

    public static PgyYearFlags FromYears(IEnumerable<int> pgyYears) =>
        pgyYears.Aggregate(PgyYearFlags.None, (flags, year) => flags | FromYear(year));

    extension(PgyYearFlags flags)
    {
        public IReadOnlyList<int> ToYears() =>
            Enum.GetValues<PgyYearFlags>()
                .Where(f => f != PgyYearFlags.None && flags.HasFlag(f))
                .Select(f => f switch
                {
                    PgyYearFlags.Pgy1 => 1,
                    PgyYearFlags.Pgy2 => 2,
                    PgyYearFlags.Pgy3 => 3,
                    PgyYearFlags.Pgy4 => 4,
                    _ => throw new ArgumentOutOfRangeException(nameof(f), $"Unhandled flag: {f}")
                })
                .ToList();

        public bool ContainsYear(int pgyYear) =>
            flags.HasFlag(FromYear(pgyYear));
    }
}