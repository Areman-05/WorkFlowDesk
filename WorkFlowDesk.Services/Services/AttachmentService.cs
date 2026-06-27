using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Common.Configuration;
using WorkFlowDesk.Data;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;

namespace WorkFlowDesk.Services.Services;

public class AttachmentService : IAttachmentService
{
    private readonly ApplicationDbContext _context;
    private readonly IActivityLogService _activityLog;

    public AttachmentService(ApplicationDbContext context, IActivityLogService activityLog)
    {
        _context = context;
        _activityLog = activityLog;
    }

    public async Task<IReadOnlyList<TareaAdjuntoInfo>> GetByTareaAsync(int tareaId)
    {
        var items = await _context.TareaAdjuntos.AsNoTracking()
            .Where(a => a.TareaId == tareaId)
            .OrderByDescending(a => a.FechaSubida)
            .ToListAsync();
        return items.Select(a => Map(a)).ToList();
    }

    public async Task<TareaAdjuntoInfo> AgregarAsync(int tareaId, string rutaOrigen, int? empleadoId)
    {
        if (!File.Exists(rutaOrigen))
            throw new FileNotFoundException("No se encontró el archivo.", rutaOrigen);

        var nombre = Path.GetFileName(rutaOrigen);
        var dir = Path.Combine(DatabasePaths.GetDataDirectory(), "attachments", tareaId.ToString());
        Directory.CreateDirectory(dir);
        var destino = Path.Combine(dir, $"{Guid.NewGuid():N}_{nombre}");
        File.Copy(rutaOrigen, destino, overwrite: true);

        var relativa = Path.GetRelativePath(DatabasePaths.GetDataDirectory(), destino);
        var info = new FileInfo(destino);
        var adjunto = new TareaAdjunto
        {
            TareaId = tareaId,
            NombreArchivo = nombre,
            RutaRelativa = relativa.Replace('\\', '/'),
            TamanoBytes = info.Length,
            FechaSubida = DateTime.Now,
            SubidoPorEmpleadoId = empleadoId
        };
        _context.TareaAdjuntos.Add(adjunto);
        await _context.SaveChangesAsync();
        await _activityLog.RegistrarAsync("Tarea", tareaId, "Adjunto", $"Archivo añadido: {nombre}");
        return Map(adjunto);
    }

    public async Task<bool> EliminarAsync(int adjuntoId)
    {
        var adjunto = await _context.TareaAdjuntos.FindAsync(adjuntoId);
        if (adjunto == null)
            return false;

        var full = ObtenerRutaCompleta(adjunto.RutaRelativa);
        if (File.Exists(full))
            File.Delete(full);

        _context.TareaAdjuntos.Remove(adjunto);
        await _context.SaveChangesAsync();
        await _activityLog.RegistrarAsync("Tarea", adjunto.TareaId, "Adjunto", $"Archivo eliminado: {adjunto.NombreArchivo}");
        return true;
    }

    public string ObtenerRutaCompleta(string rutaRelativa) =>
        Path.Combine(DatabasePaths.GetDataDirectory(), rutaRelativa.Replace('/', Path.DirectorySeparatorChar));

    private static TareaAdjuntoInfo Map(TareaAdjunto a) => new()
    {
        Id = a.Id,
        TareaId = a.TareaId,
        NombreArchivo = a.NombreArchivo,
        RutaRelativa = a.RutaRelativa,
        TamanoBytes = a.TamanoBytes,
        FechaSubida = a.FechaSubida
    };
}
