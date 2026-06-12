using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Data;
using WorkFlowDesk.Data.Seed;

namespace WorkFlowDesk.Tests.Infrastructure;

/// <summary>Crea contextos EF Core en memoria para tests de integración.</summary>
public static class TestDbContextFactory
{
    public static ApplicationDbContext Create(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    public static async Task<ApplicationDbContext> CreateSeededAsync(string? databaseName = null)
    {
        var context = Create(databaseName);
        await DatabaseSeeder.SeedAsync(context);
        return context;
    }
}
