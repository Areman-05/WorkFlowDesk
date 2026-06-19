using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Common.Helpers;
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

    /// <summary>Obtiene todos los usuarios con su rol cargado.</summary>
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

    /// <summary>Registra un usuario nuevo validando datos y asignando el rol indicado.</summary>
    public async Task<Usuario> RegistrarAsync(
        string nombreUsuario,
        string email,
        string nombreCompleto,
        string password,
        TipoRol tipoRol)
    {
        nombreUsuario = nombreUsuario.Trim();
        email = email.Trim();
        nombreCompleto = nombreCompleto.Trim();

        if (!ValidationHelper.IsValidRequired(nombreUsuario) || nombreUsuario.Length > 50)
            throw new ArgumentException("El nombre de usuario debe tener entre 1 y 50 caracteres.");

        if (!ValidationHelper.IsValidRequired(nombreCompleto) || nombreCompleto.Length > 200)
            throw new ArgumentException("El nombre completo debe tener entre 1 y 200 caracteres.");

        if (!ValidationHelper.IsValidEmail(email) || email.Length > 100)
            throw new ArgumentException("El email no tiene un formato válido.");

        if (!ValidationHelper.IsValidPassword(password))
            throw new ArgumentException("La contraseña debe tener al menos 6 caracteres.");

        if (await ExistsAsync(nombreUsuario))
            throw new InvalidOperationException("El nombre de usuario ya está en uso.");

        if (await EmailExistsAsync(email))
            throw new InvalidOperationException("El email ya está registrado.");

        var rol = await _context.Roles.FirstOrDefaultAsync(r => r.TipoRol == tipoRol && r.Activo);
        if (rol == null)
            throw new InvalidOperationException("El rol seleccionado no está disponible.");

        var usuario = new Usuario
        {
            NombreUsuario = nombreUsuario,
            Email = email,
            NombreCompleto = nombreCompleto,
            RolId = rol.Id,
            Activo = true
        };

        await CreateAsync(usuario, password);
        return (await GetByIdAsync(usuario.Id))!;
    }

    /// <summary>Actualiza los datos del usuario en la base de datos.</summary>
    public async Task UpdateAsync(Usuario usuario)
    {
        var existing = await _context.Usuarios.FindAsync(usuario.Id)
            ?? await GetByIdAsync(usuario.Id);

        if (existing == null)
            throw new InvalidOperationException($"No se encontró el usuario con ID {usuario.Id}.");

        existing.NombreUsuario = usuario.NombreUsuario;
        existing.Email = usuario.Email;
        existing.NombreCompleto = usuario.NombreCompleto;
        existing.RolId = usuario.RolId;
        existing.Activo = usuario.Activo;
        existing.UltimoAcceso = usuario.UltimoAcceso;

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

    /// <summary>Comprueba si existe un usuario con el email indicado.</summary>
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Usuarios.AnyAsync(u => u.Email == email);
    }

    /// <summary>Asigna un rol de sistema a un usuario existente.</summary>
    public async Task AsignarRolAsync(int usuarioId, TipoRol tipoRol)
    {
        var usuario = await GetByIdAsync(usuarioId)
            ?? throw new InvalidOperationException("El usuario no existe.");

        var rol = await _context.Roles.FirstOrDefaultAsync(r => r.TipoRol == tipoRol && r.Activo)
            ?? throw new InvalidOperationException("El rol seleccionado no está disponible.");

        usuario.RolId = rol.Id;
        await UpdateAsync(usuario);
    }
}
