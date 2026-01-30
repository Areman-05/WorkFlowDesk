using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Data;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;

namespace WorkFlowDesk.Services.Services;

/// <summary>Servicio de gestión de tareas.</summary>
public class TareaService : ITareaService
{
    private readonly ApplicationDbContext _context;

    public TareaService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Tarea?> GetByIdAsync(int id)
    {
        return await _context.Tareas
            .Include(t => t.Asignado)
            .Include(t => t.Creador)
            .Include(t => t.Proyecto)
            .Include(t => t.Comentarios)
                .ThenInclude(c => c.Empleado)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Tarea>> GetAllAsync()
    {
        return await _context.Tareas
            .Include(t => t.Asignado)
            .Include(t => t.Proyecto)
            .OrderByDescending(t => t.FechaCreacion)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tarea>> GetByEstadoAsync(EstadoTarea estado)
    {
        return await _context.Tareas
            .Include(t => t.Asignado)
            .Include(t => t.Proyecto)
            .Where(t => t.Estado == estado)
            .OrderByDescending(t => t.FechaCreacion)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tarea>> GetByProyectoAsync(int proyectoId)
    {
        return await _context.Tareas
            .Include(t => t.Asignado)
            .Where(t => t.ProyectoId == proyectoId)
            .OrderByDescending(t => t.FechaCreacion)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tarea>> GetByAsignadoAsync(int empleadoId)
    {
        return await _context.Tareas
            .Include(t => t.Proyecto)
            .Where(t => t.AsignadoId == empleadoId)
            .OrderByDescending(t => t.FechaCreacion)
            .ToListAsync();
    }

    public async Task<Tarea> CreateAsync(Tarea tarea)
    {
        tarea.FechaCreacion = DateTime.Now;
        _context.Tareas.Add(tarea);
        await _context.SaveChangesAsync();
        return tarea;
    }

    public async Task UpdateAsync(Tarea tarea)
    {
        _context.Tareas.Update(tarea);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var tarea = await _context.Tareas.FindAsync(id);
        if (tarea != null)
        {
            tarea.Estado = EstadoTarea.Cancelada;
            await _context.SaveChangesAsync();
        }
    }

    public async Task AgregarComentarioAsync(int tareaId, ComentarioTarea comentario)
    {
        comentario.TareaId = tareaId;
        comentario.FechaCreacion = DateTime.Now;
        _context.ComentariosTareas.Add(comentario);
        await _context.SaveChangesAsync();
    }
}
