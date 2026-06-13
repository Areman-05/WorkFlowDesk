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

    public async Task<Dictionary<string, int>> ObtenerEstadisticasEmpleadosAsync()
    {
        return new Dictionary<string, int>
        {
            { "Total", await _context.Empleados.CountAsync() },
            { "Activos", await _context.Empleados.CountAsync(e => e.Estado == EstadoEmpleado.Activo) },
            { "Inactivos", await _context.Empleados.CountAsync(e => e.Estado == EstadoEmpleado.Inactivo) },
            { "Vacaciones", await _context.Empleados.CountAsync(e => e.Estado == EstadoEmpleado.Vacaciones) },
            { "Baja", await _context.Empleados.CountAsync(e => e.Estado == EstadoEmpleado.Baja) }
        };
    }

    public async Task<Dictionary<string, int>> ObtenerEstadisticasProyectosAsync()
    {
        return new Dictionary<string, int>
        {
            { "Total", await _context.Proyectos.CountAsync() },
            { "Planificación", await _context.Proyectos.CountAsync(p => p.Estado == EstadoProyecto.Planificacion) },
            { "En Progreso", await _context.Proyectos.CountAsync(p => p.Estado == EstadoProyecto.EnProgreso) },
            { "Completados", await _context.Proyectos.CountAsync(p => p.Estado == EstadoProyecto.Completado) },
            { "Cancelados", await _context.Proyectos.CountAsync(p => p.Estado == EstadoProyecto.Cancelado) }
        };
    }

    public async Task<Dictionary<string, int>> ObtenerEstadisticasTareasAsync()
    {
        return new Dictionary<string, int>
        {
            { "Total", await _context.Tareas.CountAsync() },
            { "Pendientes", await _context.Tareas.CountAsync(t => t.Estado == EstadoTarea.Pendiente) },
            { "En Progreso", await _context.Tareas.CountAsync(t => t.Estado == EstadoTarea.EnProgreso) },
            { "Completadas", await _context.Tareas.CountAsync(t => t.Estado == EstadoTarea.Completada) },
            { "Canceladas", await _context.Tareas.CountAsync(t => t.Estado == EstadoTarea.Cancelada) }
        };
    }

    public async Task<Dictionary<string, int>> ObtenerEstadisticasClientesAsync()
    {
        return new Dictionary<string, int>
        {
            { "Total", await _context.Clientes.CountAsync() },
            { "Activos", await _context.Clientes.CountAsync(c => c.Activo) },
            { "Inactivos", await _context.Clientes.CountAsync(c => !c.Activo) }
        };
    }
}
