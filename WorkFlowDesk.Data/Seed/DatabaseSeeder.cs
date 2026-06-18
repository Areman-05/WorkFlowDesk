using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Common.Security;
using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.Data.Seed;

/// <summary>Seed de datos iniciales: roles y usuario administrador.</summary>
public static class DatabaseSeeder
{
    /// <summary>Usuario por defecto: admin / Contraseña: Admin123</summary>
    public const string DefaultAdminUserName = "admin";
    public const string DefaultAdminPassword = "Admin123";
    public const string DefaultSupervisorUserName = "supervisor";
    public const string DefaultSupervisorPassword = "Supervisor123";
    public const string DefaultEmpleadoUserName = "empleado";
    public const string DefaultEmpleadoPassword = "Empleado123";

    /// <summary>Ejecuta el seed de roles y usuarios por defecto.</summary>
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await SeedRolesAsync(context);
        await context.SaveChangesAsync();
        await SeedAdminUserAsync(context);
        await SeedDemoUserAsync(context, DefaultSupervisorUserName, "supervisor@workflowdesk.local", "Supervisor Demo", TipoRol.Supervisor, DefaultSupervisorPassword);
        await SeedDemoUserAsync(context, DefaultEmpleadoUserName, "empleado@workflowdesk.local", "Empleado Demo", TipoRol.Empleado, DefaultEmpleadoPassword);
        await context.SaveChangesAsync();

        await DemoDataSeeder.SeedAsync(context);
    }

    /// <summary>Crea el usuario administrador por defecto si no existe.</summary>
    private static async Task SeedAdminUserAsync(ApplicationDbContext context)
    {
        if (await context.Usuarios.AnyAsync(u => u.NombreUsuario == DefaultAdminUserName))
            return;

        var rolAdmin = await context.Roles.FirstOrDefaultAsync(r => r.TipoRol == TipoRol.Admin);
        if (rolAdmin == null)
            return;

        var passwordHash = PasswordHashHelper.HashPassword(DefaultAdminPassword);
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

    private static async Task SeedDemoUserAsync(
        ApplicationDbContext context,
        string nombreUsuario,
        string email,
        string nombreCompleto,
        TipoRol tipoRol,
        string password)
    {
        if (await context.Usuarios.AnyAsync(u => u.NombreUsuario == nombreUsuario))
            return;

        var rol = await context.Roles.FirstOrDefaultAsync(r => r.TipoRol == tipoRol);
        if (rol == null)
            return;

        await context.Usuarios.AddAsync(new Usuario
        {
            NombreUsuario = nombreUsuario,
            Email = email,
            PasswordHash = PasswordHashHelper.HashPassword(password),
            NombreCompleto = nombreCompleto,
            Activo = true,
            RolId = rol.Id,
            FechaCreacion = DateTime.Now
        });
    }

    /// <summary>Inserta los roles por defecto (Admin, Supervisor, Empleado) si no existen.</summary>
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
