namespace WorkFlowDesk.Common.Services;

/// <summary>Permite a los ViewModels solicitar navegación sin referenciar la capa UI.</summary>
public static class AppNavigationService
{
    public static event Action<string>? SectionRequested;

    /// <summary>Solicita navegar a una sección (Dashboard, Tareas, Perfil, etc.).</summary>
    public static void RequestSection(string sectionName) =>
        SectionRequested?.Invoke(sectionName);
}
