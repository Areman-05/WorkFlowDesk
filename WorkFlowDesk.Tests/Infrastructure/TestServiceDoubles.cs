using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;

namespace WorkFlowDesk.Tests.Infrastructure;

internal sealed class NoOpActivityLog : IActivityLogService
{
    public Task RegistrarAsync(string entidad, int? entidadId, string accion, string detalle) =>
        Task.CompletedTask;

    public Task<IReadOnlyList<ActivityLogEntry>> GetRecientesAsync(int limite = 50) =>
        Task.FromResult<IReadOnlyList<ActivityLogEntry>>(Array.Empty<ActivityLogEntry>());

    public Task<IReadOnlyList<ActivityLogEntry>> GetPorEntidadAsync(string entidad, int entidadId, int limite = 30) =>
        Task.FromResult<IReadOnlyList<ActivityLogEntry>>(Array.Empty<ActivityLogEntry>());
}

internal sealed class NoOpAutomation : IAutomationEngine
{
    public Task EvaluarTrasCambioTareaAsync(Tarea tarea) => Task.CompletedTask;
    public Task EvaluarTrasCambioProyectoAsync(Proyecto proyecto) => Task.CompletedTask;
    public Task EjecutarComprobacionesProgramadasAsync() => Task.CompletedTask;
}

internal static class TestServices
{
    public static readonly NoOpActivityLog ActivityLog = new();
    public static readonly NoOpAutomation Automation = new();
}
