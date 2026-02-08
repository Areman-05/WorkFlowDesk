using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Data;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;

namespace WorkFlowDesk.Services.Services;

/// <summary>Servicio de autenticación de usuarios.</summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasherService _passwordHasher;

    public AuthenticationService(ApplicationDbContext context, IPasswordHasherService passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    /// <summary>Valida credenciales y devuelve el usuario si son correctas; si no, null.</summary>
    public async Task<Usuario?> AutenticarAsync(string nombreUsuario, string password)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Activo);

        if (usuario == null)
            return null;

        if (!_passwordHasher.VerifyPassword(password, usuario.PasswordHash))
            return null;

        usuario.UltimoAcceso = DateTime.Now;
        await _context.SaveChangesAsync();

        return usuario;
    }

    public async Task<bool> CambiarPasswordAsync(int usuarioId, string passwordActual, string passwordNuevo)
    {
        var usuario = await _context.Usuarios.FindAsync(usuarioId);
        if (usuario == null)
            return false;

        if (!_passwordHasher.VerifyPassword(passwordActual, usuario.PasswordHash))
            return false;

        usuario.PasswordHash = _passwordHasher.HashPassword(passwordNuevo);
        await _context.SaveChangesAsync();

        return true;
    }
}
