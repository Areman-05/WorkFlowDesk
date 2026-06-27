using Microsoft.Extensions.DependencyInjection;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;

namespace WorkFlowDesk.UI.Services;

/// <summary>Genera alertas contextuales según datos reales de la aplicación.</summary>
public static class NotificationContextService
{
    public static async Task RefreshAsync(IServiceProvider services)
    {
        try
        {
            var tareaService = services.GetRequiredService<ITareaService>();
            var proyectoService = services.GetRequiredService<IProyectoService>();
            var clienteService = services.GetRequiredService<IClienteService>();

            var tareas = (await tareaService.GetAllAsync()).ToList();
            var proyectos = (await proyectoService.GetAllAsync()).ToList();
            var clientes = (await clienteService.GetAllAsync()).ToList();

            var vencidas = tareas.Count(t =>
                t.Estado != EstadoTarea.Completada &&
                t.Estado != EstadoTarea.Cancelada &&
                t.FechaVencimiento.HasValue &&
                t.FechaVencimiento.Value.Date < DateTime.Today);

            if (vencidas > 0)
            {
                InAppNotificationCenter.AddContextual(
                    vencidas == 1
                        ? "Hay 1 tarea vencida pendiente de revisión."
                        : $"Hay {vencidas} tareas vencidas pendientes de revisión.",
                    AppNotificationKind.Warning,
                    "Tareas vencidas",
                    "Tareas");
            }

            var criticas = tareas.Count(t =>
                t.Estado != EstadoTarea.Completada &&
                t.Estado != EstadoTarea.Cancelada &&
                (t.Prioridad == PrioridadTarea.Alta || t.Prioridad == PrioridadTarea.Critica));

            if (criticas > 0)
            {
                InAppNotificationCenter.AddContextual(
                    $"Tienes {criticas} tareas de prioridad alta o crítica activas.",
                    AppNotificationKind.Info,
                    "Prioridad alta",
                    "Tareas");
            }

            var pausados = proyectos.Count(p => p.Estado == EstadoProyecto.EnPausa);
            if (pausados > 0)
            {
                InAppNotificationCenter.AddContextual(
                    pausados == 1
                        ? "1 proyecto está en pausa."
                        : $"{pausados} proyectos están en pausa.",
                    AppNotificationKind.Info,
                    "Proyectos en pausa",
                    "Proyectos");
            }

            var clientesPausa = clientes.Count(c =>
                c.Activo && c.Proyectos.Any(p => p.Estado == EstadoProyecto.EnPausa));
            if (clientesPausa > 0)
            {
                InAppNotificationCenter.AddContextual(
                    "Hay clientes con proyectos pausados que podrían necesitar seguimiento.",
                    AppNotificationKind.Warning,
                    "Seguimiento comercial",
                    "Clientes");
            }
        }
        catch
        {
            // No bloquear la UI si falla la carga de alertas.
        }
    }
}
