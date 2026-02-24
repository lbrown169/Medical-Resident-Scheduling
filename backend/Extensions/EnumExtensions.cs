using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MedicalDemo.Extensions;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        DisplayAttribute? attr = value.GetType().GetField(value.ToString())?.GetCustomAttribute<DisplayAttribute>();
        return attr?.Name ?? value.ToString();
    }

    public static string? GetDisplayNameOrDefault(this Enum value)
    {
        DisplayAttribute? attr = value.GetType().GetField(value.ToString())?.GetCustomAttribute<DisplayAttribute>();
        return attr?.Name ?? null;
    }
}