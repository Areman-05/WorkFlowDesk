using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Data;

namespace WorkFlowDesk.Data.Configuration;

public static class DatabaseConfiguration
{
    public static async Task EnsureDatabaseCreatedAsync(ApplicationDbContext context)
    {
        await context.Database.EnsureCreatedAsync();
    }

    public static async Task MigrateDatabaseAsync(ApplicationDbContext context)
    {
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            await context.Database.MigrateAsync();
        }
    }

    public static async Task SeedDatabaseAsync(ApplicationDbContext context)
    {
        await Seed.DatabaseSeeder.SeedAsync(context);
    }
}
