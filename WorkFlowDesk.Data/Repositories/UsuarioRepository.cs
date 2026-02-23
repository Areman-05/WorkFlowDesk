using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Data.Repositories;
using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.Data.Repositories;

/// <summary>Repositorio de usuarios con consultas por nombre y email.</summary>
public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    /// <summary>Inicializa el repositorio con el contexto de base de datos.</summary>
    public UsuarioRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>Obtiene un usuario por nombre de usuario con su rol.</summary>
    public async Task<Usuario?> GetByNombreUsuarioAsync(string nombreUsuario)
    {
        return await _dbSet
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);
    }

    /// <summary>Obtiene un usuario por email con su rol.</summary>
    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    /// <summary>Comprueba si existe un usuario con el nombre indicado.</summary>
    public async Task<bool> ExistsByNombreUsuarioAsync(string nombreUsuario)
    {
        return await _dbSet.AnyAsync(u => u.NombreUsuario == nombreUsuario);
    }

    /// <summary>Comprueba si existe un usuario con el email indicado.</summary>
    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email);
    }
}
