using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.Services.Interfaces;

/// <summary>Extensiones de tareas: subtareas, dependencias y tiempo.</summary>
public interface ITareaExtensionService
{
    Task<IReadOnlyList<SubTarea>> GetSubtareasAsync(int tareaId);
    Task<SubTarea> AgregarSubtareaAsync(int tareaId, string titulo);
    Task ActualizarSubtareaAsync(SubTarea subtarea);
    Task EliminarSubtareaAsync(int subtareaId);

    Task<IReadOnlyList<int>> GetDependenciasAsync(int tareaId);
    Task AgregarDependenciaAsync(int tareaId, int dependeDeTareaId);
    Task EliminarDependenciaAsync(int dependenciaId);
    Task<bool> PuedeIniciarAsync(int tareaId);

    Task<IReadOnlyList<RegistroTiempo>> GetRegistrosTiempoAsync(int tareaId);
    Task<RegistroTiempo> RegistrarTiempoAsync(int tareaId, int minutos, string nota, int? empleadoId);
    Task<int> GetMinutosTotalesAsync(int tareaId);

    Task CambiarEstadoAsync(int tareaId, EstadoTarea nuevoEstado);
}
