using System.Text.RegularExpressions;

namespace WorkFlowDesk.Common.Helpers;

public static class ValidationHelper
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return EmailRegex.IsMatch(email);
    }

    public static bool IsValidPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        // Validación básica de teléfono
        return phone.Length >= 9 && phone.All(char.IsDigit);
    }

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
