using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Common.Configuration;
using WorkFlowDesk.Data;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;

namespace WorkFlowDesk.Services.Services;

public class SyncService : ISyncService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly ApplicationDbContext _context;
    private readonly string _statePath;

    public SyncService(ApplicationDbContext context)
    {
        _context = context;
        _statePath = Path.Combine(DatabasePaths.GetDataDirectory(), "sync-state.json");
        CarpetaCompartida = LoadSharedFolder();
    }

    public string? CarpetaCompartida { get; set; }

    public Task<DateTime?> GetUltimaSyncAsync()
    {
        if (!File.Exists(_statePath))
            return Task.FromResult<DateTime?>(null);

        try
        {
            var state = JsonSerializer.Deserialize<SyncState>(File.ReadAllText(_statePath));
            return Task.FromResult(state?.UltimaSyncUtc);
        }
        catch
        {
            return Task.FromResult<DateTime?>(null);
        }
    }

    public async Task<SyncResult> ExportarCambiosAsync()
    {
        if (string.IsNullOrWhiteSpace(CarpetaCompartida))
            return Fail("Configure una carpeta compartida en Configuración → Sincronización.");

        Directory.CreateDirectory(CarpetaCompartida);
        var machine = Environment.MachineName;
        var payload = new SyncPayload
        {
            Origen = machine,
            ExportadoUtc = DateTime.UtcNow,
            Tareas = await _context.Tareas.AsNoTracking().ToListAsync(),
            Proyectos = await _context.Proyectos.AsNoTracking().ToListAsync(),
            Clientes = await _context.Clientes.AsNoTracking().ToListAsync(),
            Comentarios = await _context.ComentariosTareas.AsNoTracking().ToListAsync(),
            Subtareas = await _context.Subtareas.AsNoTracking().ToListAsync(),
            Actividad = await _context.RegistrosActividad.AsNoTracking()
                .OrderByDescending(a => a.FechaUtc).Take(200).ToListAsync()
        };

        var file = Path.Combine(CarpetaCompartida, $"sync-{machine}-{DateTime.UtcNow:yyyyMMddHHmmss}.json");
        await File.WriteAllTextAsync(file, JsonSerializer.Serialize(payload, JsonOptions));
        GuardarEstado(DateTime.UtcNow);
        return Ok($"Exportado a {Path.GetFileName(file)}", payload.Tareas.Count + payload.Proyectos.Count);
    }

    public async Task<SyncResult> ImportarCambiosAsync()
    {
        if (string.IsNullOrWhiteSpace(CarpetaCompartida) || !Directory.Exists(CarpetaCompartida))
            return Fail("Carpeta compartida no disponible.");

        var files = Directory.GetFiles(CarpetaCompartida, "sync-*.json")
            .Where(f => !f.Contains(Environment.MachineName, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(File.GetCreationTimeUtc)
            .ToList();

        if (files.Count == 0)
            return Fail("No hay paquetes de otros equipos en la carpeta compartida.");

        var aplicados = 0;
        foreach (var file in files.Take(5))
        {
            var payload = JsonSerializer.Deserialize<SyncPayload>(await File.ReadAllTextAsync(file));
            if (payload == null)
                continue;

            aplicados += await MergeClientesAsync(payload.Clientes);
            aplicados += await MergeProyectosAsync(payload.Proyectos);
            aplicados += await MergeTareasAsync(payload.Tareas);
            aplicados += await MergeComentariosAsync(payload.Comentarios);
            aplicados += await MergeSubtareasAsync(payload.Subtareas);
        }

        await _context.SaveChangesAsync();
        GuardarEstado(DateTime.UtcNow);
        return Ok($"Importación completada desde {files.Count} paquete(s).", aplicados);
    }

    private async Task<int> MergeClientesAsync(List<Cliente> items)
    {
        var n = 0;
        foreach (var item in items)
        {
            var local = await _context.Clientes.FindAsync(item.Id);
            if (local == null)
            {
                _context.Clientes.Add(item);
                n++;
            }
            else if (item.FechaCreacion >= local.FechaCreacion)
            {
                _context.Entry(local).CurrentValues.SetValues(item);
                n++;
            }
        }
        return n;
    }

    private async Task<int> MergeProyectosAsync(List<Proyecto> items)
    {
        var n = 0;
        foreach (var item in items)
        {
            var local = await _context.Proyectos.FindAsync(item.Id);
            if (local == null)
            {
                _context.Proyectos.Add(item);
                n++;
            }
            else
            {
                _context.Entry(local).CurrentValues.SetValues(item);
                n++;
            }
        }
        return n;
    }

    private async Task<int> MergeTareasAsync(List<Tarea> items)
    {
        var n = 0;
        foreach (var item in items)
        {
            var local = await _context.Tareas.FindAsync(item.Id);
            if (local == null)
            {
                _context.Tareas.Add(item);
                n++;
            }
            else
            {
                _context.Entry(local).CurrentValues.SetValues(item);
                n++;
            }
        }
        return n;
    }

    private async Task<int> MergeComentariosAsync(List<ComentarioTarea> items)
    {
        var n = 0;
        foreach (var item in items)
        {
            if (await _context.ComentariosTareas.AnyAsync(c => c.Id == item.Id))
                continue;
            _context.ComentariosTareas.Add(item);
            n++;
        }
        return n;
    }

    private async Task<int> MergeSubtareasAsync(List<SubTarea> items)
    {
        var n = 0;
        foreach (var item in items)
        {
            var local = await _context.Subtareas.FindAsync(item.Id);
            if (local == null)
            {
                _context.Subtareas.Add(item);
                n++;
            }
            else
            {
                _context.Entry(local).CurrentValues.SetValues(item);
                n++;
            }
        }
        return n;
    }

    private void GuardarEstado(DateTime utc)
    {
        var state = new SyncState
        {
            UltimaSyncUtc = utc,
            CarpetaCompartida = CarpetaCompartida
        };
        File.WriteAllText(_statePath, JsonSerializer.Serialize(state, JsonOptions));
        if (!string.IsNullOrWhiteSpace(CarpetaCompartida))
            File.WriteAllText(Path.Combine(DatabasePaths.GetDataDirectory(), "sync-folder.txt"), CarpetaCompartida);
    }

    private static string? LoadSharedFolder()
    {
        var path = Path.Combine(DatabasePaths.GetDataDirectory(), "sync-folder.txt");
        return File.Exists(path) ? File.ReadAllText(path).Trim() : null;
    }

    private static SyncResult Ok(string msg, int count) => new() { Exito = true, Mensaje = msg, RegistrosAplicados = count };
    private static SyncResult Fail(string msg) => new() { Exito = false, Mensaje = msg };

    private sealed class SyncState
    {
        public DateTime UltimaSyncUtc { get; set; }
        public string? CarpetaCompartida { get; set; }
    }

    private sealed class SyncPayload
    {
        public string Origen { get; set; } = string.Empty;
        public DateTime ExportadoUtc { get; set; }
        public List<Tarea> Tareas { get; set; } = [];
        public List<Proyecto> Proyectos { get; set; } = [];
        public List<Cliente> Clientes { get; set; } = [];
        public List<ComentarioTarea> Comentarios { get; set; } = [];
        public List<SubTarea> Subtareas { get; set; } = [];
        public List<RegistroActividad> Actividad { get; set; } = [];
    }
}
