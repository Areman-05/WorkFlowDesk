using System.Windows;
using System.Windows.Input;
using WorkFlowDesk.Common.Authorization;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.UI.Helpers;
using WorkFlowDesk.UI.Services;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI;

public partial class MainWindow : Window
{
    private readonly NavigationService _navigationService;
    private readonly MainViewModel _mainViewModel;

    public MainWindow()
    {
        InitializeComponent();
        _navigationService = new NavigationService();
        _navigationService.Initialize(ContentArea);

        _mainViewModel = new MainViewModel();
        _mainViewModel.NavigateRequested += OnNavigateRequested;
        _mainViewModel.LogoutRequested += OnLogoutRequested;
        DataContext = _mainViewModel;

        AppNavigationService.SectionRequested += OnSectionRequested;
        UserPreferencesService.AvatarChanged += OnAvatarChanged;

        NavigateTo("Dashboard");
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await LoadAvatarAsync();
    }

    private void OnSectionRequested(string section)
    {
        Dispatcher.Invoke(() => NavigateTo(section));
    }

    private void OnAvatarChanged(object? sender, int userId)
    {
        if (SessionService.CurrentUser?.Id == userId)
        {
            Dispatcher.Invoke(async () =>
            {
                _mainViewModel.RefreshAvatarUrl();
                await LoadAvatarAsync();
            });
        }
    }

    private async Task LoadAvatarAsync()
    {
        _mainViewModel.RefreshAvatarUrl();
        if (string.IsNullOrEmpty(_mainViewModel.AvatarUrl))
            return;

        var image = await AvatarImageLoader.LoadAsync(_mainViewModel.AvatarUrl);
        if (image != null)
            TopBarAvatar.Source = image;
    }

    private void OnAvatarClick(object sender, MouseButtonEventArgs e)
    {
        NavigateTo("Perfil");
    }

    private void OnLogoutRequested(object? sender, EventArgs e)
    {
        if (NotificationService.ShowConfirmation(
                "¿Desea cerrar la sesión actual?",
                "Cerrar sesión") != MessageBoxResult.Yes)
        {
            return;
        }

        AppNavigationService.SectionRequested -= OnSectionRequested;
        UserPreferencesService.AvatarChanged -= OnAvatarChanged;
        AuthFlowService.LogoutAndShowLogin(this);
    }

    private void OnNavigateRequested(object? sender, string viewName)
    {
        NavigateTo(viewName);
    }

    private void NavigateTo(string viewName)
    {
        if (!CanNavigateTo(viewName))
        {
            NotificationService.ShowWarning("No tiene permisos para acceder a esta sección.");
            return;
        }

        _mainViewModel.SetActiveSection(viewName);
        _navigationService.NavigateTo(NavigationViewFactory.CreateView(viewName));
    }

    private static bool CanNavigateTo(string viewName) => viewName switch
    {
        "Dashboard" => RolePermissions.CanAccessDashboard,
        "Empleados" => RolePermissions.CanAccessEmpleados,
        "Proyectos" => RolePermissions.CanAccessProyectos,
        "Tareas" => RolePermissions.CanAccessTareas,
        "Clientes" => RolePermissions.CanAccessClientes,
        "Reportes" => RolePermissions.CanAccessReportes,
        "Configuracion" => RolePermissions.CanAccessConfiguracion,
        "Perfil" => SessionService.IsAuthenticated,
        _ => false
    };
}
