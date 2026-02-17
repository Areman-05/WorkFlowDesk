using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Data;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;

namespace WorkFlowDesk.Services.Services;

/// <summary>Servicio de gestión de usuarios.</summary>
public class UsuarioService : IUsuarioService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasherService _passwordHasher;

    /// <summary>Inicializa el servicio con el contexto y el hasher de contraseñas.</summary>
    public UsuarioService(ApplicationDbContext context, IPasswordHasherService passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    /// <summary>Obtiene un usuario por ID con su rol cargado.</summary>
    public async Task<Usuario?> GetByIdAsync(int id)
    {
        return await _context.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<IEnumerable<Usuario>> GetAllAsync()
    {
        return await _context.Usuarios
            .Include(u => u.Rol)
            .ToListAsync();
    }

    /// <summary>Crea un usuario con la contraseña hasheada.</summary>
    public async Task<Usuario> CreateAsync(Usuario usuario, string password)
    {
        usuario.PasswordHash = _passwordHasher.HashPassword(password);
        usuario.FechaCreacion = DateTime.Now;
        
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        
        return usuario;
    }

    /// <summary>Actualiza los datos del usuario en la base de datos.</summary>
    public async Task UpdateAsync(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
    }

    /// <summary>Desactiva el usuario (borrado lógico) por ID.</summary>
    public async Task DeleteAsync(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario != null)
        {
            usuario.Activo = false;
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>Comprueba si existe un usuario con el nombre indicado.</summary>
    public async Task<bool> ExistsAsync(string nombreUsuario)
    {
        return await _context.Usuarios.AnyAsync(u => u.NombreUsuario == nombreUsuario);
    }
}
