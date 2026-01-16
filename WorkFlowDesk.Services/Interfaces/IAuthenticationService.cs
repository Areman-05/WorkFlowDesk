using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.Services.Interfaces;

public interface IAuthenticationService
{
    Task<Usuario?> AutenticarAsync(string nombreUsuario, string password);
    Task<bool> CambiarPasswordAsync(int usuarioId, string passwordActual, string passwordNuevo);
}
