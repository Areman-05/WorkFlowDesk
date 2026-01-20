namespace WorkFlowDesk.Services.Interfaces;

public interface IReporteService
{
    Task<Dictionary<string, int>> ObtenerEstadisticasEmpleadosAsync();
    Task<Dictionary<string, int>> ObtenerEstadisticasProyectosAsync();
    Task<Dictionary<string, int>> ObtenerEstadisticasTareasAsync();
    Task<Dictionary<string, int>> ObtenerEstadisticasClientesAsync();
}
