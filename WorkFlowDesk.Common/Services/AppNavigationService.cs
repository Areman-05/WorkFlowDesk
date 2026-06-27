namespace WorkFlowDesk.Common.Services;

/// <summary>Permite a los ViewModels solicitar navegación sin referenciar la capa UI.</summary>
public static class AppNavigationService
{
    public static event Action<string>? SectionRequested;

    /// <summary>Proyecto pendiente de filtrar al abrir Tareas (se consume al crear el ViewModel).</summary>
    public static int? PendingProyectoFiltroId { get; set; }

    /// <summary>Solicita navegar a una sección (Dashboard, Tareas, Perfil, etc.).</summary>
    public static void RequestSection(string sectionName) =>
        SectionRequested?.Invoke(sectionName);

    /// <summary>Navega a Tareas filtrando por proyecto.</summary>
    public static void RequestTareasForProyecto(int proyectoId)
    {
        PendingProyectoFiltroId = proyectoId;
        RequestSection("Tareas");
    }
}
