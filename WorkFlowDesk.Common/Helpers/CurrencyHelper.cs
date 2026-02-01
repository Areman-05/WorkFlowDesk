using System.Globalization;

namespace WorkFlowDesk.Common.Helpers;

/// <summary>Formateo de moneda y números.</summary>
public static class CurrencyHelper
{
    private static readonly CultureInfo DefaultCulture = new("es-ES");

    public static string FormatCurrency(decimal amount, string currencySymbol = "€")
    {
        return $"{amount:N2} {currencySymbol}";
    }

    public static string FormatCurrency(decimal? amount, string currencySymbol = "€")
    {
        if (!amount.HasValue)
            return string.Empty;

        return FormatCurrency(amount.Value, currencySymbol);
    }

    public static string FormatCurrency(double amount, string currencySymbol = "€")
    {
        return FormatCurrency((decimal)amount, currencySymbol);
    }

    public static string FormatCurrency(double? amount, string currencySymbol = "€")
    {
        if (!amount.HasValue)
            return string.Empty;

        return FormatCurrency(amount.Value, currencySymbol);
    }

    public static decimal? ParseCurrency(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        // Remover símbolos de moneda y espacios
        var cleanValue = value
            .Replace("€", string.Empty)
            .Replace("$", string.Empty)
            .Replace("£", string.Empty)
            .Replace(" ", string.Empty)
            .Trim();

        if (decimal.TryParse(cleanValue, NumberStyles.Currency, DefaultCulture, out var result))
        {
            return result;
        }

        return null;
    }

    public static string FormatPercentage(decimal value, int decimals = 2)
    {
        return $"{value.ToString($"F{decimals}", DefaultCulture)}%";
    }

    public static string FormatPercentage(decimal? value, int decimals = 2)
    {
        if (!value.HasValue)
            return string.Empty;

        return FormatPercentage(value.Value, decimals);
    }
}
