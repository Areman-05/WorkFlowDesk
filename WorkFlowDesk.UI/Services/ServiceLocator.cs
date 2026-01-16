using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Data;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.Services.Services;

namespace WorkFlowDesk.UI.Services;

public static class ServiceLocator
{
    private static ServiceProvider? _serviceProvider;

    public static void ConfigureServices()
    {
        var services = new ServiceCollection();

        // Configuración
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // DbContext
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Server=(localdb)\\mssqllocaldb;Database=WorkFlowDeskDb;Trusted_Connection=True;MultipleActiveResultSets=true";
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Servicios
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUsuarioService, UsuarioService>();

        _serviceProvider = services.BuildServiceProvider();
    }

    public static T GetService<T>() where T : class
    {
        return _serviceProvider?.GetRequiredService<T>() 
            ?? throw new InvalidOperationException("ServiceProvider no está configurado");
    }
}
