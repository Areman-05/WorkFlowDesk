using WorkFlowDesk.Common.Security;
using WorkFlowDesk.Services.Interfaces;

namespace WorkFlowDesk.Services.Services;

/// <summary>Servicio de hash y verificación de contraseñas.</summary>
public class PasswordHasherService : IPasswordHasherService
{
    /// <summary>Genera un hash PBKDF2 de la contraseña.</summary>
    public string HashPassword(string password) => PasswordHashHelper.HashPassword(password);

    /// <summary>Comprueba si la contraseña coincide con el hash almacenado.</summary>
    public bool VerifyPassword(string password, string hash) =>
        PasswordHashHelper.VerifyPassword(password, hash);
}
