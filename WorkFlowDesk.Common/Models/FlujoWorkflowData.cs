namespace WorkFlowDesk.Common.Models;

/// <summary>Datos serializables del constructor de flujos y automatizaciones.</summary>
public class FlujoWorkflowData
{
    public string NombreFlujoActual { get; set; } = "Nuevo Proceso";
    public List<FlowStepData> PasosActuales { get; set; } = new();
    public List<AutomatizacionData> Automatizaciones { get; set; } = new();
}

public class FlowStepData
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Tipo { get; set; } = "Trigger";
    public string EtiquetaCorta { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}

public class AutomatizacionData
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Trigger { get; set; } = string.Empty;
    public string UltimaEjecucion { get; set; } = string.Empty;
    public string Icono { get; set; } = "\uE7C4";
    public string IconoColor { get; set; } = "#9E00B5";
    public bool Activa { get; set; } = true;
    public List<FlowStepData> Pasos { get; set; } = new();
}
