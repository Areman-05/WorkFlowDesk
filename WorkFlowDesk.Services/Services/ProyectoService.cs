using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Data;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;

namespace WorkFlowDesk.Services.Services;

/// <summary>Servicio de gestión de proyectos.</summary>
public class ProyectoService : IProyectoService
{
    private readonly ApplicationDbContext _context;

    public ProyectoService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>Obtiene un proyecto por ID con cliente y responsable cargados.</summary>
    public async Task<Proyecto?> GetByIdAsync(int id)
    {
        return await _context.Proyectos
            .Include(p => p.Cliente)
            .Include(p => p.Responsable)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Proyecto>> GetAllAsync()
    {
        return await _context.Proyectos
            .Include(p => p.Cliente)
            .Include(p => p.Responsable)
            .OrderByDescending(p => p.FechaCreacion)
            .ToListAsync();
    }

    public async Task<IEnumerable<Proyecto>> GetByEstadoAsync(EstadoProyecto estado)
    {
        return await _context.Proyectos
            .Include(p => p.Cliente)
            .Include(p => p.Responsable)
            .Where(p => p.Estado == estado)
            .OrderByDescending(p => p.FechaCreacion)
            .ToListAsync();
    }

    public async Task<Proyecto> CreateAsync(Proyecto proyecto)
    {
        proyecto.FechaCreacion = DateTime.Now;
        _context.Proyectos.Add(proyecto);
        await _context.SaveChangesAsync();
        return proyecto;
    }

    public async Task UpdateAsync(Proyecto proyecto)
    {
        _context.Proyectos.Update(proyecto);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var proyecto = await _context.Proyectos.FindAsync(id);
        if (proyecto != null)
        {
            proyecto.Estado = EstadoProyecto.Cancelado;
            await _context.SaveChangesAsync();
        }
    }
}
