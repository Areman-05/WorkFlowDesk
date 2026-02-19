using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Data;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.Services.Services;

namespace WorkFlowDesk.UI.Services;

/// <summary>Contenedor de servicios de la aplicación (DI).</summary>
public static class ServiceLocator
{
    private static ServiceProvider? _serviceProvider;

    /// <summary>Registra todos los servicios y construye el proveedor de dependencias.</summary>
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
        services.AddScoped<IEmpleadoService, EmpleadoService>();
        services.AddScoped<IProyectoService, ProyectoService>();
        services.AddScoped<ITareaService, TareaService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IReporteService, ReporteService>();
        services.AddScoped<IDatabaseInitializationService, DatabaseInitializationService>();
        services.AddScoped<IExportService, ExportService>();
        services.AddScoped<IBackupService, BackupService>();

        _serviceProvider = services.BuildServiceProvider();
    }

    /// <summary>Obtiene un servicio registrado. Requiere haber llamado antes a <see cref="ConfigureServices"/>.</summary>
    public static T GetService<T>() where T : class
    {
        return _serviceProvider?.GetRequiredService<T>() 
            ?? throw new InvalidOperationException("ServiceProvider no está configurado");
    }
}
