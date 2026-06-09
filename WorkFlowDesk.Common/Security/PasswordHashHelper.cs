using System.Security.Cryptography;
using System.Text;

namespace WorkFlowDesk.Common.Security;

/// <summary>Hash y verificación de contraseñas con PBKDF2; compatible con hashes SHA256 legacy.</summary>
public static class PasswordHashHelper
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;
    private const string Pbkdf2Prefix = "PBKDF2";

    /// <summary>Genera un hash PBKDF2 con salt aleatorio.</summary>
    public static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize);

        return $"{Pbkdf2Prefix}.{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    /// <summary>Verifica la contraseña contra un hash PBKDF2 o SHA256 legacy.</summary>
    public static bool VerifyPassword(string password, string storedHash)
    {
        if (storedHash.StartsWith($"{Pbkdf2Prefix}.", StringComparison.Ordinal))
        {
            return VerifyPbkdf2(password, storedHash);
        }

        return VerifyLegacySha256(password, storedHash);
    }

    /// <summary>Indica si el hash almacenado usa el formato legacy SHA256.</summary>
    public static bool IsLegacyHash(string storedHash) =>
        !storedHash.StartsWith($"{Pbkdf2Prefix}.", StringComparison.Ordinal);

    private static bool VerifyPbkdf2(string password, string storedHash)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 4)
            return false;

        if (!int.TryParse(parts[1], out var iterations))
            return false;

        var salt = Convert.FromBase64String(parts[2]);
        var expectedHash = Convert.FromBase64String(parts[3]);
        var actualHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            expectedHash.Length);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }

    private static bool VerifyLegacySha256(string password, string storedHash)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        var hashOfInput = Convert.ToBase64String(hashedBytes);
        return hashOfInput == storedHash;
    }
}
