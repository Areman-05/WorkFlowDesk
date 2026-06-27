using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Authorization;
using WorkFlowDesk.Common.Configuration;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.Services.Interfaces;
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

    private IListToolbarProvider? _activeToolbar;
    private INotifyPropertyChanged? _activeToolbarNotifiable;
    private bool _isNotificationPanelOpen;
    private bool _isGlobalSearchOpen;
    private string _globalSearchText = string.Empty;
    private readonly IGlobalSearchService? _globalSearchService;

    public MainViewModel(IGlobalSearchService? globalSearchService = null)
    {
        _globalSearchService = globalSearchService;
        RefreshAvatarUrl();
        InAppNotificationCenter.Changed += (_, _) => NotifyNotificationState();

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
        ToggleNotificationPanelCommand = new RelayCommand(() => IsNotificationPanelOpen = !IsNotificationPanelOpen);
        MarcarNotificacionesLeidasCommand = new RelayCommand(MarcarNotificacionesLeidas);
        EliminarNotificacionCommand = new RelayCommand<AppNotificationItem>(EliminarNotificacion);
        EliminarTodasNotificacionesCommand = new RelayCommand(EliminarTodasNotificaciones);
        NavegarDesdeNotificacionCommand = new RelayCommand<AppNotificationItem>(NavegarDesdeNotificacion);
        OpenGlobalSearchCommand = new RelayCommand(OpenGlobalSearch);
        CloseGlobalSearchCommand = new RelayCommand(CloseGlobalSearch);
        ExecuteGlobalSearchCommand = new AsyncRelayCommand(ExecuteGlobalSearchAsync);
        SelectGlobalSearchResultCommand = new RelayCommand<GlobalSearchResult>(SelectGlobalSearchResult);
    }

    public string ProductTagline => AppProductInfo.Tagline;

    public ObservableCollection<GlobalSearchResult> GlobalSearchResults { get; } = new();

    public bool IsGlobalSearchOpen
    {
        get => _isGlobalSearchOpen;
        set => SetProperty(ref _isGlobalSearchOpen, value);
    }

    public string GlobalSearchText
    {
        get => _globalSearchText;
        set
        {
            if (!SetProperty(ref _globalSearchText, value))
                return;

            _ = ExecuteGlobalSearchCommand.ExecuteAsync(null);
        }
    }

    public ObservableCollection<AppNotificationItem> Notificaciones => InAppNotificationCenter.Items;

    public bool TieneNotificacionesNoLeidas => InAppNotificationCenter.NoLeidas > 0;

    public bool HayNotificaciones => InAppNotificationCenter.Items.Count > 0;

    public bool IsNotificationPanelOpen
    {
        get => _isNotificationPanelOpen;
        set => SetProperty(ref _isNotificationPanelOpen, value);
    }

    public bool IsToolbarVisible => _activeToolbar != null;

    public bool ToolbarExportVisible => _activeToolbar?.ToolbarExportVisible ?? false;

    public bool ToolbarCreateVisible => _activeToolbar?.ToolbarCreateVisible ?? false;

    public string ToolbarCreateLabel => _activeToolbar?.ToolbarCreateLabel ?? "Nuevo";

    public IAsyncRelayCommand? ToolbarExportCommand => _activeToolbar?.ToolbarExportCommand;

    public IRelayCommand? ToolbarCreateCommand => _activeToolbar?.ToolbarCreateCommand;

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
        "Perfil" => "Buscar en perfil...",
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
    public IRelayCommand ToggleNotificationPanelCommand { get; }
    public IRelayCommand MarcarNotificacionesLeidasCommand { get; }
    public IRelayCommand<AppNotificationItem> EliminarNotificacionCommand { get; }
    public IRelayCommand EliminarTodasNotificacionesCommand { get; }
    public IRelayCommand<AppNotificationItem> NavegarDesdeNotificacionCommand { get; }
    public IRelayCommand OpenGlobalSearchCommand { get; }
    public IRelayCommand CloseGlobalSearchCommand { get; }
    public IAsyncRelayCommand ExecuteGlobalSearchCommand { get; }
    public IRelayCommand<GlobalSearchResult> SelectGlobalSearchResultCommand { get; }

    public event EventHandler<string>? NavigateRequested;
    public event EventHandler? LogoutRequested;
    public event EventHandler<string>? NotificationNavigationRequested;

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

    /// <summary>Enlaza la barra de acciones con la vista activa.</summary>
    public void SetActiveToolbar(object? dataContext)
    {
        if (_activeToolbarNotifiable != null)
            _activeToolbarNotifiable.PropertyChanged -= OnActiveToolbarPropertyChanged;

        _activeToolbar = dataContext as IListToolbarProvider;
        _activeToolbarNotifiable = dataContext as INotifyPropertyChanged;

        if (_activeToolbarNotifiable != null)
            _activeToolbarNotifiable.PropertyChanged += OnActiveToolbarPropertyChanged;

        NotifyToolbarState();
    }

    private void OnActiveToolbarPropertyChanged(object? sender, PropertyChangedEventArgs e) =>
        NotifyToolbarState();

    private void NotifyToolbarState()
    {
        OnPropertyChanged(nameof(IsToolbarVisible));
        OnPropertyChanged(nameof(ToolbarExportVisible));
        OnPropertyChanged(nameof(ToolbarCreateVisible));
        OnPropertyChanged(nameof(ToolbarCreateLabel));
        OnPropertyChanged(nameof(ToolbarExportCommand));
        OnPropertyChanged(nameof(ToolbarCreateCommand));
    }

    private void NotifyNotificationState()
    {
        OnPropertyChanged(nameof(TieneNotificacionesNoLeidas));
        OnPropertyChanged(nameof(HayNotificaciones));
        OnPropertyChanged(nameof(Notificaciones));
    }

    private void MarcarNotificacionesLeidas()
    {
        InAppNotificationCenter.MarkAllRead();
        IsNotificationPanelOpen = false;
    }

    private void EliminarNotificacion(AppNotificationItem? item)
    {
        if (item == null)
            return;

        InAppNotificationCenter.Remove(item);
    }

    private void EliminarTodasNotificaciones()
    {
        InAppNotificationCenter.RemoveAll();
        IsNotificationPanelOpen = false;
    }

    private void NavegarDesdeNotificacion(AppNotificationItem? item)
    {
        if (item == null)
            return;

        InAppNotificationCenter.MarkRead(item);
        IsNotificationPanelOpen = false;

        if (!string.IsNullOrWhiteSpace(item.Seccion))
            NotificationNavigationRequested?.Invoke(this, item.Seccion);
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

    private void OpenGlobalSearch()
    {
        IsNotificationPanelOpen = false;
        _globalSearchText = string.Empty;
        OnPropertyChanged(nameof(GlobalSearchText));
        GlobalSearchResults.Clear();
        IsGlobalSearchOpen = true;
    }

    private void CloseGlobalSearch()
    {
        IsGlobalSearchOpen = false;
        _globalSearchText = string.Empty;
        OnPropertyChanged(nameof(GlobalSearchText));
        GlobalSearchResults.Clear();
    }

    private async Task ExecuteGlobalSearchAsync()
    {
        if (_globalSearchService == null)
            return;

        if (string.IsNullOrWhiteSpace(GlobalSearchText))
        {
            GlobalSearchResults.Clear();
            return;
        }

        var results = await _globalSearchService.BuscarAsync(GlobalSearchText);
        GlobalSearchResults.Clear();
        foreach (var result in results)
            GlobalSearchResults.Add(result);
    }

    private void SelectGlobalSearchResult(GlobalSearchResult? result)
    {
        if (result == null || string.IsNullOrWhiteSpace(result.Seccion))
            return;

        CloseGlobalSearch();
        Navigate(result.Seccion);
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
