using System.Text.RegularExpressions;

namespace WorkFlowDesk.Common.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string? value)
    {
        return string.IsNullOrEmpty(value);
    }

    public static bool IsNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    public static string ToSafeString(this string? value, string defaultValue = "")
    {
        return value ?? defaultValue;
    }

    public static string Truncate(this string value, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            return value;

        return value.Substring(0, maxLength - suffix.Length) + suffix;
    }

    public static string RemoveWhitespace(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return Regex.Replace(value, @"\s+", string.Empty);
    }

    public static string CapitalizeFirst(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return char.ToUpper(value[0]) + value.Substring(1).ToLower();
    }

    public static bool ContainsIgnoreCase(this string source, string value)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(value))
            return false;

        return source.Contains(value, StringComparison.OrdinalIgnoreCase);
    }

    public static string FormatWith(this string format, params object[] args)
    {
        return string.Format(format, args);
    }
}
