using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Authorization;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>ViewModel principal con comandos de navegación al sidebar.</summary>
public class MainViewModel : ViewModelBase
{
    private string _userName = SessionService.GetUserName();
    private string _userRole = SessionService.GetUserRole();
    private string _currentSection = "Dashboard";
    private string _avatarUrl = string.Empty;

    public MainViewModel()
    {
        RefreshAvatarUrl();

        NavigateToDashboardCommand = new RelayCommand(() => Navigate("Dashboard"));
        NavigateToEmpleadosCommand = new RelayCommand(() => Navigate("Empleados"));
        NavigateToProyectosCommand = new RelayCommand(() => Navigate("Proyectos"));
        NavigateToTareasCommand = new RelayCommand(() => Navigate("Tareas"));
        NavigateToClientesCommand = new RelayCommand(() => Navigate("Clientes"));
        NavigateToReportesCommand = new RelayCommand(() => Navigate("Reportes"));
        NavigateToConfiguracionCommand = new RelayCommand(() => Navigate("Configuracion"));
        NavigateToPerfilCommand = new RelayCommand(() => Navigate("Perfil"));
        LogoutCommand = new RelayCommand(() => LogoutRequested?.Invoke(this, EventArgs.Empty));
    }

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

    public string CurrentSection
    {
        get => _currentSection;
        set => SetProperty(ref _currentSection, value);
    }

    public string AvatarUrl
    {
        get => _avatarUrl;
        set => SetProperty(ref _avatarUrl, value);
    }

    public bool ShowEmpleados => RolePermissions.CanAccessEmpleados;
    public bool ShowProyectos => RolePermissions.CanAccessProyectos;
    public bool ShowClientes => RolePermissions.CanAccessClientes;
    public bool ShowReportes => RolePermissions.CanAccessReportes;
    public bool ShowConfiguracion => RolePermissions.CanAccessConfiguracion;

    public bool IsDashboardActive => CurrentSection == "Dashboard";
    public bool IsEmpleadosActive => CurrentSection == "Empleados";
    public bool IsProyectosActive => CurrentSection == "Proyectos";
    public bool IsTareasActive => CurrentSection == "Tareas";
    public bool IsClientesActive => CurrentSection == "Clientes";
    public bool IsReportesActive => CurrentSection == "Reportes";
    public bool IsConfiguracionActive => CurrentSection == "Configuracion";
    public bool IsPerfilActive => CurrentSection == "Perfil";

    public IRelayCommand NavigateToDashboardCommand { get; }
    public IRelayCommand NavigateToEmpleadosCommand { get; }
    public IRelayCommand NavigateToProyectosCommand { get; }
    public IRelayCommand NavigateToTareasCommand { get; }
    public IRelayCommand NavigateToClientesCommand { get; }
    public IRelayCommand NavigateToReportesCommand { get; }
    public IRelayCommand NavigateToConfiguracionCommand { get; }
    public IRelayCommand NavigateToPerfilCommand { get; }
    public IRelayCommand LogoutCommand { get; }

    public event EventHandler<string>? NavigateRequested;
    public event EventHandler? LogoutRequested;

    /// <summary>Actualiza la URL del avatar según preferencias del usuario.</summary>
    public void RefreshAvatarUrl()
    {
        var user = SessionService.CurrentUser;
        if (user == null)
        {
            AvatarUrl = string.Empty;
            return;
        }

        var index = UserPreferencesService.GetAvatarIndex(user.Id);
        AvatarUrl = AvatarCatalog.GetUrl(index);
    }

    private void Navigate(string section)
    {
        CurrentSection = section;
        NotifyNavigationState();
        NavigateRequested?.Invoke(this, section);
    }

    /// <summary>Actualiza la sección activa del menú lateral.</summary>
    public void SetActiveSection(string section)
    {
        CurrentSection = section;
        NotifyNavigationState();
    }

    private void NotifyNavigationState()
    {
        OnPropertyChanged(nameof(IsDashboardActive));
        OnPropertyChanged(nameof(IsEmpleadosActive));
        OnPropertyChanged(nameof(IsProyectosActive));
        OnPropertyChanged(nameof(IsTareasActive));
        OnPropertyChanged(nameof(IsClientesActive));
        OnPropertyChanged(nameof(IsReportesActive));
        OnPropertyChanged(nameof(IsConfiguracionActive));
        OnPropertyChanged(nameof(IsPerfilActive));
    }
}
