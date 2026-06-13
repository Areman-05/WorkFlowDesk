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

    /// <summary>Aplica las migraciones pendientes hasta el esquema más reciente.</summary>
    public static async Task MigrateDatabaseAsync(ApplicationDbContext context)
    {
        await context.Database.MigrateAsync();
    }

    /// <summary>Ejecuta el seed de datos iniciales (roles, usuario admin).</summary>
    public static async Task SeedDatabaseAsync(ApplicationDbContext context)
    {
        await Seed.DatabaseSeeder.SeedAsync(context);
    }

    /// <summary>Comprueba si una tabla existe en SQLite.</summary>
    public static async Task<bool> TableExistsAsync(ApplicationDbContext context, string tableName)
    {
        var connection = context.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = $name";
        var parameter = command.CreateParameter();
        parameter.ParameterName = "$name";
        parameter.Value = tableName;
        command.Parameters.Add(parameter);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result) > 0;
    }
}
