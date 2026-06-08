using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>ViewModel principal con comandos de navegación al sidebar.</summary>
public class MainViewModel : ViewModelBase
{
    private string _userName = SessionService.GetUserName();
    private string _userRole = SessionService.GetUserRole();

    public string UserName
    {
        get => _userName;
        set => SetProperty(ref _userName, value);
    }

    public string UserRole
    {
        get => _userRole;
        set => SetProperty(ref _userRole, value);
    }

    public IRelayCommand NavigateToDashboardCommand { get; }
    public IRelayCommand NavigateToEmpleadosCommand { get; }
    public IRelayCommand NavigateToProyectosCommand { get; }
    public IRelayCommand NavigateToTareasCommand { get; }
    public IRelayCommand NavigateToClientesCommand { get; }
    public IRelayCommand NavigateToReportesCommand { get; }
    public IRelayCommand NavigateToConfiguracionCommand { get; }
    public IRelayCommand LogoutCommand { get; }

    public event EventHandler<string>? NavigateRequested;
    public event EventHandler? LogoutRequested;

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
        LogoutCommand = new RelayCommand(() => LogoutRequested?.Invoke(this, EventArgs.Empty));
    }
}
