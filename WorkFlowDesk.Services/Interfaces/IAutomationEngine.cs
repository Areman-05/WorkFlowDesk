using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.Services.Interfaces;

/// <summary>Motor de automatizaciones configurables (reglas reales).</summary>
public interface IAutomationEngine
{
    Task EvaluarTrasCambioTareaAsync(Tarea tarea);
    Task EvaluarTrasCambioProyectoAsync(Proyecto proyecto);
    Task EjecutarComprobacionesProgramadasAsync();
}
