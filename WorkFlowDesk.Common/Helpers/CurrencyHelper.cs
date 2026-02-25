using System.Globalization;

namespace WorkFlowDesk.Common.Helpers;

/// <summary>Formateo de moneda y números.</summary>
public static class CurrencyHelper
{
    private static readonly CultureInfo DefaultCulture = new("es-ES");

    /// <summary>Formatea un importe como moneda con símbolo (p. ej. 1.234,56 €).</summary>
    public static string FormatCurrency(decimal amount, string currencySymbol = "€")
    {
        return $"{amount:N2} {currencySymbol}";
    }

    /// <summary>Formatea un importe nullable; devuelve cadena vacía si es null.</summary>
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

    /// <summary>Intenta parsear una cadena como importe (acepta €, $, espacios).</summary>
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

    /// <summary>Formatea un valor como porcentaje (p. ej. 12,50%).</summary>
    public static string FormatPercentage(decimal value, int decimals = 2)
    {
        return $"{value.ToString($"F{decimals}", DefaultCulture)}%";
    }

    /// <summary>Formatea un porcentaje nullable; cadena vacía si es null.</summary>
    public static string FormatPercentage(decimal? value, int decimals = 2)
    {
        if (!value.HasValue)
            return string.Empty;

        return FormatPercentage(value.Value, decimals);
    }
}
