using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.Data.Seed;

public static class DatabaseSeeder
{
    /// <summary>Usuario por defecto: admin / Contraseña: Admin123</summary>
    public const string DefaultAdminUserName = "admin";
    public const string DefaultAdminPassword = "Admin123";

    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await SeedRolesAsync(context);
        await SeedAdminUserAsync(context);
        await context.SaveChangesAsync();
    }

    private static async Task SeedAdminUserAsync(ApplicationDbContext context)
    {
        if (await context.Usuarios.AnyAsync(u => u.NombreUsuario == DefaultAdminUserName))
            return;

        var rolAdmin = await context.Roles.FirstOrDefaultAsync(r => r.TipoRol == TipoRol.Admin);
        if (rolAdmin == null)
            return;

        var passwordHash = HashPassword(DefaultAdminPassword);
        var adminUsuario = new Usuario
        {
            NombreUsuario = DefaultAdminUserName,
            Email = "admin@workflowdesk.local",
            PasswordHash = passwordHash,
            NombreCompleto = "Administrador",
            Activo = true,
            RolId = rolAdmin.Id,
            FechaCreacion = DateTime.Now
        };

        await context.Usuarios.AddAsync(adminUsuario);
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private static async Task SeedRolesAsync(ApplicationDbContext context)
    {
        if (await context.Roles.AnyAsync())
            return;

        var roles = new List<Rol>
        {
            new Rol
            {
                TipoRol = TipoRol.Admin,
                Nombre = "Administrador",
                Descripcion = "Acceso completo al sistema",
                FechaCreacion = DateTime.Now,
                Activo = true
            },
            new Rol
            {
                TipoRol = TipoRol.Supervisor,
                Nombre = "Supervisor",
                Descripcion = "Puede gestionar proyectos y tareas",
                FechaCreacion = DateTime.Now,
                Activo = true
            },
            new Rol
            {
                TipoRol = TipoRol.Empleado,
                Nombre = "Empleado",
                Descripcion = "Acceso básico al sistema",
                FechaCreacion = DateTime.Now,
                Activo = true
            }
        };

        await context.Roles.AddRangeAsync(roles);
    }
}
