using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.Services.Interfaces;

/// <summary>Servicio de autenticación de usuarios.</summary>
public interface IAuthenticationService
{
    /// <summary>Autentica al usuario con nombre y contraseña; devuelve null si falla.</summary>
    Task<Usuario?> AutenticarAsync(string nombreUsuario, string password);
    Task<bool> CambiarPasswordAsync(int usuarioId, string passwordActual, string passwordNuevo);
}
