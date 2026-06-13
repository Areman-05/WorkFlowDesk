using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using WorkFlowDesk.Common.Configuration;

namespace WorkFlowDesk.Data;

/// <summary>Fábrica de diseño para migraciones EF Core (dotnet ef).</summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlite(DatabasePaths.GetConnectionString());
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
