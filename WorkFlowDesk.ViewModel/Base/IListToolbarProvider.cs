using CommunityToolkit.Mvvm.Input;

namespace WorkFlowDesk.ViewModel.Base;

/// <summary>Barra de acciones (exportar / crear) para la top bar principal.</summary>
public interface IListToolbarProvider
{
    bool ToolbarExportVisible { get; }
    bool ToolbarCreateVisible { get; }
    string ToolbarCreateLabel { get; }
    IAsyncRelayCommand? ToolbarExportCommand { get; }
    IRelayCommand? ToolbarCreateCommand { get; }
}
