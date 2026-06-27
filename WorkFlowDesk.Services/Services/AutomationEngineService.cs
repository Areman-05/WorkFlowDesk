using WorkFlowDesk.Common.Models;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;

namespace WorkFlowDesk.Services.Services;

/// <summary>Ejecuta automatizaciones activas definidas en Optimización.</summary>
public class AutomationEngineService : IAutomationEngine
{
    private readonly IIntegrationService _integration;
    private readonly IActivityLogService _activityLog;

    public AutomationEngineService(IIntegrationService integration, IActivityLogService activityLog)
    {
        _integration = integration;
        _activityLog = activityLog;
    }

    public async Task EvaluarTrasCambioTareaAsync(Tarea tarea)
    {
        var autos = FlujoWorkflowService.Load().Automatizaciones.Where(a => a.Activa).ToList();
        foreach (var auto in autos)
            await EvaluarAutomatizacionTareaAsync(auto, tarea);
    }

    public async Task EvaluarTrasCambioProyectoAsync(Proyecto proyecto)
    {
        var autos = FlujoWorkflowService.Load().Automatizaciones.Where(a => a.Activa).ToList();
        foreach (var auto in autos.Where(a => a.Id == "cierre-proyecto"))
        {
            if (proyecto.Estado == EstadoProyecto.Completado)
            {
                var msg = $"Proyecto completado: {proyecto.Nombre}";
                InAppNotificationCenter.Add(msg, AppNotificationKind.Info, "Automatización");
                await _integration.EnviarEmailAsync("cliente@workflowdesk.local", "Proyecto finalizado", msg);
                await _activityLog.RegistrarAsync("Proyecto", proyecto.Id, "Automatización", msg);
            }
        }
    }

    public async Task EjecutarComprobacionesProgramadasAsync()
    {
        var autos = FlujoWorkflowService.Load().Automatizaciones.Where(a => a.Activa).ToList();
        if (autos.Any(a => a.Id is "escalado-tareas" or "recordatorios-flujo"))
            await EvaluarRetrasosYRecordatoriosAsync();
    }

    private async Task EvaluarAutomatizacionTareaAsync(AutomatizacionData auto, Tarea tarea)
    {
        switch (auto.Id)
        {
            case "escalado-tareas":
                if (EsTareaRetrasada(tarea))
                {
                    var msg = $"Tarea retrasada: {tarea.Titulo}";
                    InAppNotificationCenter.Add(msg, AppNotificationKind.Warning, "Escalado automático", "Tareas");
                    await _activityLog.RegistrarAsync("Tarea", tarea.Id, "Automatización", msg);
                }
                break;
            case "sync-slack":
                var slackMsg = $"Tarea actualizada: {tarea.Titulo} ({tarea.Estado})";
                await _integration.NotificarSlackAsync(slackMsg);
                break;
        }
    }

    private async Task EvaluarRetrasosYRecordatoriosAsync()
    {
        // Las comprobaciones periódicas se invocan al abrir la app / dashboard.
        await _activityLog.RegistrarAsync("Sistema", null, "Automatización", "Comprobación programada ejecutada");
    }

    private static bool EsTareaRetrasada(Tarea t) =>
        t.FechaVencimiento.HasValue &&
        t.FechaVencimiento.Value.Date < DateTime.Today &&
        t.Estado is not EstadoTarea.Completada and not EstadoTarea.Cancelada;
}
