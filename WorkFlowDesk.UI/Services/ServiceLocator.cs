using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Common.Configuration;
using WorkFlowDesk.Data;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.Services.Services;

namespace WorkFlowDesk.UI.Services;

/// <summary>Contenedor de servicios de la aplicación (DI).</summary>
public static class ServiceLocator
{
    private static ServiceProvider? _rootProvider;
    private static IServiceScope? _appScope;

    /// <summary>Proveedor de dependencias del scope activo de la aplicación.</summary>
    public static IServiceProvider Provider =>
        _appScope?.ServiceProvider
        ?? throw new InvalidOperationException("ServiceProvider no está configurado. Llame a ConfigureServices() primero.");

    /// <summary>Registra servicios y abre un scope único para toda la sesión de la app.</summary>
    public static void ConfigureServices()
    {
        var services = new ServiceCollection();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        var configured = configuration.GetConnectionString("DefaultConnection");
        var connectionString = string.IsNullOrWhiteSpace(configured)
            ? DatabasePaths.GetConnectionString()
            : configured;

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlite(connectionString);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
        }, ServiceLifetime.Scoped);

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
        services.AddScoped<IActivityLogService, ActivityLogService>();
        services.AddScoped<IGlobalSearchService, GlobalSearchService>();
        services.AddScoped<IAttachmentService, AttachmentService>();
        services.AddScoped<ISyncService, SyncService>();
        services.AddScoped<IIntegrationService, IntegrationService>();
        services.AddScoped<IAutomationEngine, AutomationEngineService>();
        services.AddScoped<ITareaExtensionService, TareaExtensionService>();
        services.AddScoped<IBackupService, BackupService>();

        _rootProvider?.Dispose();
        _appScope?.Dispose();

        _rootProvider = services.BuildServiceProvider();
        _appScope = _rootProvider.CreateScope();
    }

    /// <summary>Obtiene un servicio registrado del scope activo.</summary>
    public static T GetService<T>() where T : class => Provider.GetRequiredService<T>();

    /// <summary>Libera recursos al cerrar la aplicación.</summary>
    public static void Dispose()
    {
        _appScope?.Dispose();
        _appScope = null;
        _rootProvider?.Dispose();
        _rootProvider = null;
    }
}
