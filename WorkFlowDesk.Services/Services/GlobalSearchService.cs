using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Data;
using WorkFlowDesk.Services.Interfaces;

namespace WorkFlowDesk.Services.Services;

public class GlobalSearchService : IGlobalSearchService
{
    private readonly ApplicationDbContext _context;

    public GlobalSearchService(ApplicationDbContext context) => _context = context;

    public async Task<IReadOnlyList<GlobalSearchResult>> BuscarAsync(string termino, int limite = 20)
    {
        if (string.IsNullOrWhiteSpace(termino))
            return Array.Empty<GlobalSearchResult>();

        var q = termino.Trim();
        var results = new List<GlobalSearchResult>();

        var tareas = await _context.Tareas.AsNoTracking()
            .Where(t => t.Titulo.Contains(q) || t.Descripcion.Contains(q))
            .OrderByDescending(t => t.FechaCreacion)
            .Take(limite)
            .Select(t => new GlobalSearchResult
            {
                Tipo = "Tarea",
                Id = t.Id,
                Titulo = t.Titulo,
                Subtitulo = t.Estado.ToString(),
                Seccion = "Tareas"
            }).ToListAsync();
        results.AddRange(tareas);

        var proyectos = await _context.Proyectos.AsNoTracking()
            .Where(p => p.Nombre.Contains(q) || p.Descripcion.Contains(q))
            .Take(limite)
            .Select(p => new GlobalSearchResult
            {
                Tipo = "Proyecto",
                Id = p.Id,
                Titulo = p.Nombre,
                Subtitulo = p.Estado.ToString(),
                Seccion = "Proyectos"
            }).ToListAsync();
        results.AddRange(proyectos);

        var clientes = await _context.Clientes.AsNoTracking()
            .Where(c => c.Nombre.Contains(q) || c.Email.Contains(q) || c.Empresa.Contains(q))
            .Take(limite)
            .Select(c => new GlobalSearchResult
            {
                Tipo = "Cliente",
                Id = c.Id,
                Titulo = c.Nombre,
                Subtitulo = c.Email,
                Seccion = "Clientes"
            }).ToListAsync();
        results.AddRange(clientes);

        var empleados = await _context.Empleados.AsNoTracking()
            .Where(e => e.Nombre.Contains(q) || e.Apellidos.Contains(q) || e.Email.Contains(q))
            .Take(limite)
            .Select(e => new GlobalSearchResult
            {
                Tipo = "Empleado",
                Id = e.Id,
                Titulo = e.Nombre + " " + e.Apellidos,
                Subtitulo = e.Email,
                Seccion = "Empleados"
            }).ToListAsync();
        results.AddRange(empleados);

        return results.Take(limite).ToList();
    }
}
