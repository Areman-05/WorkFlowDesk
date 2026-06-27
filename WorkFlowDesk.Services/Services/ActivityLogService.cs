using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.Data;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;

namespace WorkFlowDesk.Services.Services;

public class ActivityLogService : IActivityLogService
{
    private readonly ApplicationDbContext _context;

    public ActivityLogService(ApplicationDbContext context) => _context = context;

    public async Task RegistrarAsync(string entidad, int? entidadId, string accion, string detalle)
    {
        var user = SessionService.CurrentUser;
        _context.RegistrosActividad.Add(new RegistroActividad
        {
            FechaUtc = DateTime.UtcNow,
            UsuarioId = user?.Id,
            EmpleadoId = user?.Id,
            Entidad = entidad,
            EntidadId = entidadId,
            Accion = accion,
            Detalle = detalle
        });
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<ActivityLogEntry>> GetRecientesAsync(int limite = 50) =>
        await QueryEntries().Take(limite).ToListAsync();

    public async Task<IReadOnlyList<ActivityLogEntry>> GetPorEntidadAsync(string entidad, int entidadId, int limite = 30) =>
        await QueryEntries()
            .Where(r => r.Entidad == entidad && r.EntidadId == entidadId)
            .Take(limite)
            .ToListAsync();

    private IQueryable<ActivityLogEntry> QueryEntries() =>
        _context.RegistrosActividad
            .AsNoTracking()
            .Include(r => r.Usuario)
            .OrderByDescending(r => r.FechaUtc)
            .Select(r => new ActivityLogEntry
            {
                Id = r.Id,
                FechaUtc = r.FechaUtc,
                Usuario = r.Usuario != null ? r.Usuario.NombreCompleto : "Sistema",
                Entidad = r.Entidad,
                EntidadId = r.EntidadId,
                Accion = r.Accion,
                Detalle = r.Detalle
            });
}
