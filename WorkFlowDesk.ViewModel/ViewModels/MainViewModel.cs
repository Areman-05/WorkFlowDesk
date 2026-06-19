using System.ComponentModel;
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
    private ISearchableViewModel? _activeSearchable;
    private INotifyPropertyChanged? _activeSearchNotifiable;
    private bool _syncingSearchFromChild;
    private string _textoBusqueda = string.Empty;

    public MainViewModel()
    {
        RefreshAvatarUrl();

        NavigateToDashboardCommand = new RelayCommand(() => Navigate("Dashboard"));
        NavigateToEmpleadosCommand = new RelayCommand(() => Navigate("Empleados"));
        NavigateToProyectosCommand = new RelayCommand(() => Navigate("Proyectos"));
        NavigateToTareasCommand = new RelayCommand(() => Navigate("Tareas"));
        NavigateToClientesCommand = new RelayCommand(() => Navigate("Clientes"));
        NavigateToReportesCommand = new RelayCommand(() => Navigate("Reportes"));
        NavigateToOptimizacionCommand = new RelayCommand(() => Navigate("Optimizacion"));
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

    public int AvatarIndex
    {
        get
        {
            var user = SessionService.CurrentUser;
            return user == null ? 0 : UserPreferencesService.GetAvatarIndex(user.Id);
        }
    }

    public string UserInitials
    {
        get
        {
            var user = SessionService.CurrentUser;
            if (user == null || string.IsNullOrWhiteSpace(user.NombreCompleto))
                return "?";

            var parts = user.NombreCompleto.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
                return $"{parts[0][0]}{parts[^1][0]}".ToUpperInvariant();

            return parts[0][0].ToString().ToUpperInvariant();
        }
    }

    public bool ShowEmpleados => RolePermissions.CanAccessEmpleados;
    public bool ShowProyectos => RolePermissions.CanAccessProyectos;
    public bool ShowClientes => RolePermissions.CanAccessClientes;
    public bool ShowReportes => RolePermissions.CanAccessReportes;
    public bool ShowOptimizacion => RolePermissions.CanAccessOptimizacion;
    public bool ShowConfiguracion => RolePermissions.CanAccessConfiguracion;

    public bool IsDashboardActive => CurrentSection == "Dashboard";
    public bool IsEmpleadosActive => CurrentSection == "Empleados";
    public bool IsProyectosActive => CurrentSection == "Proyectos";
    public bool IsTareasActive => CurrentSection == "Tareas";
    public bool IsClientesActive => CurrentSection == "Clientes";
    public bool IsReportesActive => CurrentSection == "Reportes";
    public bool IsOptimizacionActive => CurrentSection == "Optimizacion";
    public bool IsConfiguracionActive => CurrentSection == "Configuracion";
    public bool IsPerfilActive => CurrentSection == "Perfil";

    public bool IsSearchVisible => _activeSearchable != null;

    public string SearchPlaceholder => CurrentSection switch
    {
        "Empleados" => "Buscar empleados...",
        "Proyectos" => "Buscar proyectos, clientes...",
        "Tareas" => "Buscar tareas...",
        "Clientes" => "Buscar clientes...",
        "Optimizacion" => "Buscar procesos o flujos...",
        "Configuracion" => "Buscar parámetros...",
        _ => "Buscar..."
    };

    public string TextoBusqueda
    {
        get => _textoBusqueda;
        set
        {
            if (!SetProperty(ref _textoBusqueda, value))
                return;

            if (_syncingSearchFromChild || _activeSearchable == null)
                return;

            _activeSearchable.TextoBusqueda = value;
        }
    }

    public IRelayCommand NavigateToDashboardCommand { get; }
    public IRelayCommand NavigateToEmpleadosCommand { get; }
    public IRelayCommand NavigateToProyectosCommand { get; }
    public IRelayCommand NavigateToTareasCommand { get; }
    public IRelayCommand NavigateToClientesCommand { get; }
    public IRelayCommand NavigateToReportesCommand { get; }
    public IRelayCommand NavigateToOptimizacionCommand { get; }
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
        OnPropertyChanged(nameof(AvatarIndex));
        OnPropertyChanged(nameof(UserInitials));
    }

    /// <summary>Enlaza el buscador superior con la vista activa.</summary>
    public void SetActiveSearchable(object? dataContext)
    {
        if (_activeSearchNotifiable != null)
            _activeSearchNotifiable.PropertyChanged -= OnActiveSearchPropertyChanged;

        _activeSearchable = dataContext as ISearchableViewModel;
        _activeSearchNotifiable = dataContext as INotifyPropertyChanged;

        if (_activeSearchNotifiable != null)
            _activeSearchNotifiable.PropertyChanged += OnActiveSearchPropertyChanged;

        _syncingSearchFromChild = true;
        _textoBusqueda = _activeSearchable?.TextoBusqueda ?? string.Empty;
        OnPropertyChanged(nameof(TextoBusqueda));
        _syncingSearchFromChild = false;

        OnPropertyChanged(nameof(IsSearchVisible));
        OnPropertyChanged(nameof(SearchPlaceholder));
    }

    private void OnActiveSearchPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(TextoBusqueda) || _activeSearchable == null)
            return;

        var childText = _activeSearchable.TextoBusqueda ?? string.Empty;
        if (_textoBusqueda == childText)
            return;

        _syncingSearchFromChild = true;
        SetProperty(ref _textoBusqueda, childText, nameof(TextoBusqueda));
        _syncingSearchFromChild = false;
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
        OnPropertyChanged(nameof(IsOptimizacionActive));
        OnPropertyChanged(nameof(IsConfiguracionActive));
        OnPropertyChanged(nameof(IsPerfilActive));
    }
}
