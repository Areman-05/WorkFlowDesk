using System.Text.RegularExpressions;

namespace WorkFlowDesk.Common.Helpers;

/// <summary>Validación de email, teléfono y otros formatos.</summary>
public static class ValidationHelper
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    /// <summary>Valida que la cadena sea un email con formato correcto.</summary>
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return EmailRegex.IsMatch(email);
    }

    /// <summary>Valida que el teléfono tenga al menos 9 dígitos.</summary>
    public static bool IsValidPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        // Validación básica de teléfono
        return phone.Length >= 9 && phone.All(char.IsDigit);
    }

    /// <summary>Valida que la contraseña tenga al menos 6 caracteres.</summary>
    public static bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        // Mínimo 6 caracteres
        return password.Length >= 6;
    }

    public static bool IsValidRequired(string value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }
}
