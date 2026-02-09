using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Data;

namespace WorkFlowDesk.Data.Configuration;

/// <summary>Utilidades para crear, migrar y preparar la base de datos.</summary>
public static class DatabaseConfiguration
{
    /// <summary>Crea la base de datos si no existe.</summary>
    public static async Task EnsureDatabaseCreatedAsync(ApplicationDbContext context)
    {
        await context.Database.EnsureCreatedAsync();
    }

    /// <summary>Aplica las migraciones pendientes a la base de datos.</summary>
    public static async Task MigrateDatabaseAsync(ApplicationDbContext context)
    {
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            await context.Database.MigrateAsync();
        }
    }

    /// <summary>Ejecuta el seed de datos iniciales (roles, usuario admin).</summary>
    public static async Task SeedDatabaseAsync(ApplicationDbContext context)
    {
        await Seed.DatabaseSeeder.SeedAsync(context);
    }
}
