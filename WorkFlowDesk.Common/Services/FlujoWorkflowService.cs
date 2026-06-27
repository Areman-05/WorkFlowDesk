using System.Text.Json;
using WorkFlowDesk.Common.Configuration;
using WorkFlowDesk.Common.Models;

namespace WorkFlowDesk.Common.Services;

/// <summary>Persistencia local de flujos y automatizaciones.</summary>
public static class FlujoWorkflowService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private static FlujoWorkflowData? _data;

    internal static string? StorageDirectoryOverride { get; set; }

    public static FlujoWorkflowData Load()
    {
        if (_data != null)
            return _data;

        var path = GetStorePath();
        if (File.Exists(path))
        {
            try
            {
                var json = File.ReadAllText(path);
                _data = JsonSerializer.Deserialize<FlujoWorkflowData>(json) ?? CrearDatosPorDefecto();
            }
            catch
            {
                _data = CrearDatosPorDefecto();
            }
        }
        else
        {
            _data = CrearDatosPorDefecto();
        }

        return _data;
    }

    public static void Save(FlujoWorkflowData data)
    {
        _data = data;
        var path = GetStorePath();
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, JsonSerializer.Serialize(data, JsonOptions));
    }

    public static void Reset()
    {
        _data = null;
        var path = GetStorePath();
        if (File.Exists(path))
            File.Delete(path);
    }

    internal static void InvalidateCache() => _data = null;

    private static string GetStorePath() =>
        Path.Combine(StorageDirectoryOverride ?? DatabasePaths.GetDataDirectory(), "flujos-workflow.json");

    private static FlujoWorkflowData CrearDatosPorDefecto() => new()
    {
        NombreFlujoActual = "Nuevo Proceso",
        PasosActuales =
        [
            new FlowStepData
            {
                Tipo = "Trigger",
                EtiquetaCorta = "Cada vez que...",
                Descripcion = "Se crea una nueva tarea"
            },
            new FlowStepData
            {
                Tipo = "Condicion",
                EtiquetaCorta = "Si...",
                Descripcion = "Prioridad es \"Alta\""
            },
            new FlowStepData
            {
                Tipo = "Accion",
                EtiquetaCorta = "Entonces...",
                Descripcion = "Asignar a PM Senior"
            }
        ],
        Automatizaciones = CrearAutomatizacionesPorDefecto()
    };

    private static List<AutomatizacionData> CrearAutomatizacionesPorDefecto() =>
    [
        new()
        {
            Id = "cierre-proyecto",
            Nombre = "Notificación de cierre de Proyecto",
            Descripcion = "Envía correo a cliente al finalizar",
            Trigger = "Estado → Completado",
            UltimaEjecucion = "Hace 2 horas",
            Icono = "\uE7C4",
            IconoColor = "#9E00B5",
            Activa = true
        },
        new()
        {
            Id = "escalado-tareas",
            Nombre = "Escalado de Tareas Vencidas",
            Descripcion = "Asigna a supervisor tras 24h de atraso",
            Trigger = "Cron (Daily)",
            UltimaEjecucion = "Ayer, 09:00 AM",
            Icono = "\uE7BA",
            IconoColor = "#944A00",
            Activa = true
        },
        new()
        {
            Id = "sync-slack",
            Nombre = "Sync con Slack",
            Descripcion = "Espeja actualizaciones en canal #ventas",
            Trigger = "Webhook Externo",
            UltimaEjecucion = "Hace 15 min",
            Icono = "\uE8BD",
            IconoColor = "#87466D",
            Activa = false
        },
        new()
        {
            Id = "backup-semanal",
            Nombre = "Respaldo Semanal de Datos",
            Descripcion = "Exporta CSV de tareas cada lunes",
            Trigger = "Cron (Weekly)",
            UltimaEjecucion = "Hace 3 días",
            Icono = "\uE895",
            IconoColor = "#9E00B5",
            Activa = true
        }
    ];
}
