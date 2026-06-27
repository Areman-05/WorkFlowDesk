using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Data;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;

namespace WorkFlowDesk.Services.Services;

public class TareaExtensionService : ITareaExtensionService
{
    private readonly ApplicationDbContext _context;
    private readonly IActivityLogService _activityLog;
    private readonly IAutomationEngine _automation;

    public TareaExtensionService(
        ApplicationDbContext context,
        IActivityLogService activityLog,
        IAutomationEngine automation)
    {
        _context = context;
        _activityLog = activityLog;
        _automation = automation;
    }

    public async Task<IReadOnlyList<SubTarea>> GetSubtareasAsync(int tareaId) =>
        await _context.Subtareas.AsNoTracking()
            .Where(s => s.TareaId == tareaId)
            .OrderBy(s => s.Orden)
            .ToListAsync();

    public async Task<SubTarea> AgregarSubtareaAsync(int tareaId, string titulo)
    {
        var maxOrden = await _context.Subtareas.Where(s => s.TareaId == tareaId).MaxAsync(s => (int?)s.Orden) ?? 0;
        var st = new SubTarea { TareaId = tareaId, Titulo = titulo.Trim(), Orden = maxOrden + 1 };
        _context.Subtareas.Add(st);
        await _context.SaveChangesAsync();
        await _activityLog.RegistrarAsync("Tarea", tareaId, "Subtarea", $"Añadida: {titulo}");
        return st;
    }

    public async Task ActualizarSubtareaAsync(SubTarea subtarea)
    {
        _context.Subtareas.Update(subtarea);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarSubtareaAsync(int subtareaId)
    {
        var st = await _context.Subtareas.FindAsync(subtareaId);
        if (st == null) return;
        _context.Subtareas.Remove(st);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<int>> GetDependenciasAsync(int tareaId) =>
        await _context.TareaDependencias.AsNoTracking()
            .Where(d => d.TareaId == tareaId)
            .Select(d => d.DependeDeTareaId)
            .ToListAsync();

    public async Task AgregarDependenciaAsync(int tareaId, int dependeDeTareaId)
    {
        if (tareaId == dependeDeTareaId)
            throw new InvalidOperationException("Una tarea no puede depender de sí misma.");

        if (await _context.TareaDependencias.AnyAsync(d => d.TareaId == tareaId && d.DependeDeTareaId == dependeDeTareaId))
            return;

        _context.TareaDependencias.Add(new TareaDependencia
        {
            TareaId = tareaId,
            DependeDeTareaId = dependeDeTareaId
        });
        await _context.SaveChangesAsync();
        await _activityLog.RegistrarAsync("Tarea", tareaId, "Dependencia", $"Depende de tarea #{dependeDeTareaId}");
    }

    public async Task EliminarDependenciaAsync(int dependenciaId)
    {
        var dep = await _context.TareaDependencias.FindAsync(dependenciaId);
        if (dep == null) return;
        _context.TareaDependencias.Remove(dep);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> PuedeIniciarAsync(int tareaId)
    {
        var deps = await _context.TareaDependencias
            .Where(d => d.TareaId == tareaId)
            .Select(d => d.DependeDeTareaId)
            .ToListAsync();

        if (deps.Count == 0)
            return true;

        var bloqueantes = await _context.Tareas
            .Where(t => deps.Contains(t.Id))
            .Where(t => t.Estado != EstadoTarea.Completada)
            .CountAsync();

        return bloqueantes == 0;
    }

    public async Task<IReadOnlyList<RegistroTiempo>> GetRegistrosTiempoAsync(int tareaId) =>
        await _context.RegistrosTiempo.AsNoTracking()
            .Include(r => r.Empleado)
            .Where(r => r.TareaId == tareaId)
            .OrderByDescending(r => r.Fecha)
            .ToListAsync();

    public async Task<RegistroTiempo> RegistrarTiempoAsync(int tareaId, int minutos, string nota, int? empleadoId)
    {
        var reg = new RegistroTiempo
        {
            TareaId = tareaId,
            Minutos = minutos,
            Nota = nota,
            EmpleadoId = empleadoId,
            Fecha = DateTime.Now
        };
        _context.RegistrosTiempo.Add(reg);
        await _context.SaveChangesAsync();
        await _activityLog.RegistrarAsync("Tarea", tareaId, "Tiempo", $"{minutos} min registrados");
        return reg;
    }

    public async Task<int> GetMinutosTotalesAsync(int tareaId) =>
        await _context.RegistrosTiempo.Where(r => r.TareaId == tareaId).SumAsync(r => r.Minutos);

    public async Task CambiarEstadoAsync(int tareaId, EstadoTarea nuevoEstado)
    {
        if (nuevoEstado == EstadoTarea.EnProgreso && !await PuedeIniciarAsync(tareaId))
            throw new InvalidOperationException("Hay dependencias sin completar.");

        var tarea = await _context.Tareas.FindAsync(tareaId)
            ?? throw new InvalidOperationException("Tarea no encontrada.");

        var anterior = tarea.Estado;
        tarea.Estado = nuevoEstado;
        await _context.SaveChangesAsync();
        await _activityLog.RegistrarAsync("Tarea", tareaId, "Estado", $"{anterior} → {nuevoEstado}");
        await _automation.EvaluarTrasCambioTareaAsync(tarea);
    }
}
