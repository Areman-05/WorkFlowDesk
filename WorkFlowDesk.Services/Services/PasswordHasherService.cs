using System.Security.Cryptography;
using System.Text;
using WorkFlowDesk.Services.Interfaces;

namespace WorkFlowDesk.Services.Services;

/// <summary>Servicio de hash y verificación de contraseñas.</summary>
public class PasswordHasherService : IPasswordHasherService
{
    /// <summary>Genera el hash SHA256 en Base64 de la contraseña.</summary>
    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    /// <summary>Comprueba si la contraseña coincide con el hash almacenado.</summary>
    public bool VerifyPassword(string password, string hash)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput == hash;
    }
}
