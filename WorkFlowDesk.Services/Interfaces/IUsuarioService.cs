using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.Services.Interfaces;

public interface IUsuarioService
{
    Task<Usuario?> GetByIdAsync(int id);
    Task<IEnumerable<Usuario>> GetAllAsync();
    Task<Usuario> CreateAsync(Usuario usuario, string password);
    Task<Usuario> RegistrarAsync(string nombreUsuario, string email, string nombreCompleto, string password, TipoRol tipoRol);
    Task UpdateAsync(Usuario usuario);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(string nombreUsuario);
    Task<bool> EmailExistsAsync(string email);
    Task AsignarRolAsync(int usuarioId, TipoRol tipoRol);
}
