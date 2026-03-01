using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.Data.Repositories;

/// <summary>Repositorio de usuarios con búsqueda por nombre y email.</summary>
public interface IUsuarioRepository : IRepository<Usuario>
{
    /// <summary>Obtiene un usuario por su nombre de usuario.</summary>
    Task<Usuario?> GetByNombreUsuarioAsync(string nombreUsuario);
    Task<Usuario?> GetByEmailAsync(string email);
    Task<bool> ExistsByNombreUsuarioAsync(string nombreUsuario);
    Task<bool> ExistsByEmailAsync(string email);
}
