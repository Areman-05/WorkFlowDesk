using Microsoft.Extensions.DependencyInjection;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.UI.Helpers;

namespace WorkFlowDesk.UI.Services;

/// <summary>Precarga datos del espacio de trabajo mientras la splash anima la barra por segmentos.</summary>
public static class WorkspaceBootstrapService
{
    public static async Task BootstrapPostLoginAsync(CancellationToken cancellationToken = default)
    {
        await RunBootstrapAsync(minTotalMs: 4800, includeAlerts: true, cancellationToken);
    }

    public static async Task BootstrapQuickRestoreAsync(CancellationToken cancellationToken = default)
    {
        await RunBootstrapAsync(minTotalMs: 1600, includeAlerts: false, cancellationToken);
    }

    private static async Task RunBootstrapAsync(int minTotalMs, bool includeAlerts, CancellationToken cancellationToken)
    {
        var started = Environment.TickCount64;
        var services = ServiceLocator.Provider;

        await Task.WhenAll(
            AvatarImageLoader.PreloadCatalogAsync(),
            WarmWorkspaceCacheAsync(services, cancellationToken),
            includeAlerts ? NotificationContextService.RefreshAsync(services) : Task.CompletedTask,
            EjecutarAutomatizacionesAsync(services, cancellationToken),
            PreloadCurrentUserAvatarAsync(cancellationToken)).WaitAsync(cancellationToken);

        var elapsed = Environment.TickCount64 - started;
        var remaining = minTotalMs - elapsed;
        if (remaining > 0)
            await Task.Delay((int)remaining, cancellationToken);

        await Task.Delay(280, cancellationToken);
    }

    private static async Task WarmWorkspaceCacheAsync(IServiceProvider services, CancellationToken cancellationToken)
    {
        var empleadoService = services.GetRequiredService<IEmpleadoService>();
        var proyectoService = services.GetRequiredService<IProyectoService>();
        var tareaService = services.GetRequiredService<ITareaService>();
        var clienteService = services.GetRequiredService<IClienteService>();

        await Task.WhenAll(
            empleadoService.GetAllAsync(),
            proyectoService.GetAllAsync(),
            tareaService.GetAllAsync(),
            clienteService.GetAllAsync()).WaitAsync(cancellationToken);
    }

    private static async Task EjecutarAutomatizacionesAsync(IServiceProvider services, CancellationToken cancellationToken)
    {
        var automation = services.GetRequiredService<IAutomationEngine>();
        var tareaService = services.GetRequiredService<ITareaService>();
        await automation.EjecutarComprobacionesProgramadasAsync().WaitAsync(cancellationToken);

        var hoy = DateTime.Today;
        var tareas = await tareaService.GetAllAsync().WaitAsync(cancellationToken);
        foreach (var tarea in tareas.Where(t =>
                     t.FechaVencimiento?.Date == hoy.AddDays(2) &&
                     t.Estado is not EstadoTarea.Completada and not EstadoTarea.Cancelada))
        {
            await automation.EvaluarTrasCambioTareaAsync(tarea).WaitAsync(cancellationToken);
        }
    }

    private static async Task PreloadCurrentUserAvatarAsync(CancellationToken cancellationToken)
    {
        var userId = SessionService.CurrentUser?.Id;
        if (userId == null)
            return;

        var avatarIndex = UserPreferencesService.GetAvatarIndex(userId.Value);
        var avatarUrl = AvatarCatalog.GetUrl(avatarIndex);
        await AvatarImageLoader.LoadAsync(avatarUrl).WaitAsync(cancellationToken);
    }
}
