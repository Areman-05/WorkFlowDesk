using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace WorkFlowDesk.Common.Helpers;

/// <summary>Formateo de texto (mayúsculas, título, truncar, etc.).</summary>
public static class TextFormatter
{
    /// <summary>Pone la primera letra en mayúscula y el resto en minúsculas.</summary>
    public static string CapitalizeFirstLetter(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        return char.ToUpper(text[0]) + text.Substring(1).ToLower();
    }

    /// <summary>Convierte el texto a formato título (primera letra de cada palabra en mayúscula).</summary>
    public static string ToTitleCase(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        var textInfo = new CultureInfo("es-ES", false).TextInfo;
        return textInfo.ToTitleCase(text.ToLower());
    }

    /// <summary>Elimina acentos y diacríticos del texto.</summary>
    public static string RemoveAccents(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    /// <summary>Trunca el texto al máximo de caracteres y añade sufijo si aplica.</summary>
    public static string Truncate(string text, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrWhiteSpace(text) || text.Length <= maxLength)
            return text;

        return text.Substring(0, maxLength - suffix.Length) + suffix;
    }

    /// <summary>Elimina caracteres que no sean letras, números o espacios.</summary>
    public static string RemoveSpecialCharacters(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        return Regex.Replace(text, @"[^a-zA-Z0-9\s]", string.Empty);
    }

    /// <summary>Formatea un número de 9 dígitos como XXX XXX XXX.</summary>
    public static string FormatPhoneNumber(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return phone;

        var digits = new string(phone.Where(char.IsDigit).ToArray());
        
        if (digits.Length == 9)
        {
            return $"{digits.Substring(0, 3)} {digits.Substring(3, 3)} {digits.Substring(6)}";
        }

        return phone;
    }
}
