using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Data;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;

namespace WorkFlowDesk.Services.Services;

/// <summary>Servicio de reportes y estadísticas.</summary>
public class ReporteService : IReporteService
{
    private readonly ApplicationDbContext _context;

    public ReporteService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>Obtiene conteos de empleados por estado (Total, Activos, Inactivos, etc.).</summary>
    public async Task<Dictionary<string, int>> ObtenerEstadisticasEmpleadosAsync()
    {
        var empleados = await _context.Empleados.ToListAsync();
        
        return new Dictionary<string, int>
        {
            { "Total", empleados.Count },
            { "Activos", empleados.Count(e => e.Estado == EstadoEmpleado.Activo) },
            { "Inactivos", empleados.Count(e => e.Estado == EstadoEmpleado.Inactivo) },
            { "Vacaciones", empleados.Count(e => e.Estado == EstadoEmpleado.Vacaciones) },
            { "Baja", empleados.Count(e => e.Estado == EstadoEmpleado.Baja) }
        };
    }

    /// <summary>Obtiene conteos de proyectos por estado.</summary>
    public async Task<Dictionary<string, int>> ObtenerEstadisticasProyectosAsync()
    {
        var proyectos = await _context.Proyectos.ToListAsync();
        
        return new Dictionary<string, int>
        {
            { "Total", proyectos.Count },
            { "Planificación", proyectos.Count(p => p.Estado == EstadoProyecto.Planificacion) },
            { "En Progreso", proyectos.Count(p => p.Estado == EstadoProyecto.EnProgreso) },
            { "Completados", proyectos.Count(p => p.Estado == EstadoProyecto.Completado) },
            { "Cancelados", proyectos.Count(p => p.Estado == EstadoProyecto.Cancelado) }
        };
    }

    /// <summary>Obtiene conteos de tareas por estado y prioridad.</summary>
    public async Task<Dictionary<string, int>> ObtenerEstadisticasTareasAsync()
    {
        var tareas = await _context.Tareas.ToListAsync();
        
        return new Dictionary<string, int>
        {
            { "Total", tareas.Count },
            { "Pendientes", tareas.Count(t => t.Estado == EstadoTarea.Pendiente) },
            { "En Progreso", tareas.Count(t => t.Estado == EstadoTarea.EnProgreso) },
            { "Completadas", tareas.Count(t => t.Estado == EstadoTarea.Completada) },
            { "Canceladas", tareas.Count(t => t.Estado == EstadoTarea.Cancelada) }
        };
    }

    /// <summary>Obtiene total de clientes y clientes activos.</summary>
    public async Task<Dictionary<string, int>> ObtenerEstadisticasClientesAsync()
    {
        var clientes = await _context.Clientes.ToListAsync();
        
        return new Dictionary<string, int>
        {
            { "Total", clientes.Count },
            { "Activos", clientes.Count(c => c.Activo) },
            { "Inactivos", clientes.Count(c => !c.Activo) }
        };
    }
}
