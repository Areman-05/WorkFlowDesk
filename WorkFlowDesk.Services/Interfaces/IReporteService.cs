namespace WorkFlowDesk.Services.Interfaces;

/// <summary>Servicio de reportes y estadísticas (empleados, proyectos, tareas, clientes).</summary>
public interface IReporteService
{
    Task<Dictionary<string, int>> ObtenerEstadisticasEmpleadosAsync();
    Task<Dictionary<string, int>> ObtenerEstadisticasProyectosAsync();
    Task<Dictionary<string, int>> ObtenerEstadisticasTareasAsync();
    Task<Dictionary<string, int>> ObtenerEstadisticasClientesAsync();
}
