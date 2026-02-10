using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Data;
using WorkFlowDesk.Data.Configuration;
using WorkFlowDesk.Data.Seed;
using WorkFlowDesk.Services.Interfaces;

namespace WorkFlowDesk.Services.Services;

/// <summary>Servicio de inicialización y seed de la base de datos.</summary>
public class DatabaseInitializationService : IDatabaseInitializationService
{
    private readonly ApplicationDbContext _context;

    public DatabaseInitializationService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>Crea la base de datos si no existe y ejecuta el seed inicial.</summary>
    public async Task InitializeAsync()
    {
        try
        {
            await DatabaseConfiguration.EnsureDatabaseCreatedAsync(_context);
            await DatabaseConfiguration.SeedDatabaseAsync(_context);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al inicializar la base de datos: {ex.Message}", ex);
        }
    }

    /// <summary>Ejecuta solo el seed de datos (roles, admin); no crea la BD.</summary>
    public async Task SeedDataAsync()
    {
        try
        {
            await DatabaseSeeder.SeedAsync(_context);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al poblar datos iniciales: {ex.Message}", ex);
        }
    }
}
