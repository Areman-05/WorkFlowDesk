using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.Data.Repositories;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> GetByNombreUsuarioAsync(string nombreUsuario);
    Task<Usuario?> GetByEmailAsync(string email);
    Task<bool> ExistsByNombreUsuarioAsync(string nombreUsuario);
    Task<bool> ExistsByEmailAsync(string email);
}
