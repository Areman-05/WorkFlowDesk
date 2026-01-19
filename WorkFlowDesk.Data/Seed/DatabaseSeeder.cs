using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.Data.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await SeedRolesAsync(context);
        await context.SaveChangesAsync();
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
