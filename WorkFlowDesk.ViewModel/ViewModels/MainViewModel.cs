using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>ViewModel principal con comandos de navegación al sidebar.</summary>
public class MainViewModel : ViewModelBase
{
    public IRelayCommand NavigateToDashboardCommand { get; }
    public IRelayCommand NavigateToEmpleadosCommand { get; }
    public IRelayCommand NavigateToProyectosCommand { get; }
    public IRelayCommand NavigateToTareasCommand { get; }
    public IRelayCommand NavigateToClientesCommand { get; }
    public IRelayCommand NavigateToReportesCommand { get; }
    public IRelayCommand NavigateToConfiguracionCommand { get; }

    public event EventHandler<string>? NavigateRequested;

    /// <summary>Registra los comandos de navegación del menú lateral.</summary>
    public MainViewModel()
    {
        NavigateToDashboardCommand = new RelayCommand(() => NavigateRequested?.Invoke(this, "Dashboard"));
        NavigateToEmpleadosCommand = new RelayCommand(() => NavigateRequested?.Invoke(this, "Empleados"));
        NavigateToProyectosCommand = new RelayCommand(() => NavigateRequested?.Invoke(this, "Proyectos"));
        NavigateToTareasCommand = new RelayCommand(() => NavigateRequested?.Invoke(this, "Tareas"));
        NavigateToClientesCommand = new RelayCommand(() => NavigateRequested?.Invoke(this, "Clientes"));
        NavigateToReportesCommand = new RelayCommand(() => NavigateRequested?.Invoke(this, "Reportes"));
        NavigateToConfiguracionCommand = new RelayCommand(() => NavigateRequested?.Invoke(this, "Configuracion"));
    }
}
