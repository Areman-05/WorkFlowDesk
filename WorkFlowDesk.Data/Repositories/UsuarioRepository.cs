using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Data.Repositories;
using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.Data.Repositories;

/// <summary>Repositorio de usuarios con consultas por nombre y email.</summary>
public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Usuario?> GetByNombreUsuarioAsync(string nombreUsuario)
    {
        return await _dbSet
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> ExistsByNombreUsuarioAsync(string nombreUsuario)
    {
        return await _dbSet.AnyAsync(u => u.NombreUsuario == nombreUsuario);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email);
    }
}
