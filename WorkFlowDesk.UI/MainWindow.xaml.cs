using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
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
    private DispatcherTimer? _toastTimer;

    public MainWindow()
    {
        InitializeComponent();
        _navigationService = new NavigationService();
        _navigationService.Initialize(ContentArea);

        _mainViewModel = new MainViewModel();
        _mainViewModel.NavigateRequested += OnNavigateRequested;
        _mainViewModel.LogoutRequested += OnLogoutRequested;
        _mainViewModel.NotificationNavigationRequested += OnNotificationNavigationRequested;
        DataContext = _mainViewModel;

        NotificationPopup.Closed += (_, _) => _mainViewModel.IsNotificationPanelOpen = false;

        AppNavigationService.SectionRequested += OnSectionRequested;
        UserPreferencesService.AvatarChanged += OnAvatarChanged;
        NotificationService.NotificationShown += OnAppNotificationShown;

        NavigateTo("Dashboard");
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        _mainViewModel.RefreshAvatarUrl();
        await AvatarImageLoader.PreloadCatalogAsync();
        await NotificationContextService.RefreshAsync(ServiceLocator.Provider);
    }

    private void OnSectionRequested(string section)
    {
        Dispatcher.Invoke(() => NavigateTo(section));
    }

    private void OnAvatarChanged(object? sender, int userId)
    {
        if (SessionService.CurrentUser?.Id == userId)
            Dispatcher.Invoke(() => _mainViewModel.RefreshAvatarUrl());
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
        NotificationService.NotificationShown -= OnAppNotificationShown;
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
        var view = NavigationViewFactory.CreateView(viewName);
        _mainViewModel.SetActiveSearchable(view.DataContext);
        _mainViewModel.SetActiveToolbar(view.DataContext);
        _navigationService.NavigateTo(view);

        _ = NotificationContextService.RefreshAsync(ServiceLocator.Provider);
    }

    private void OnNotificationNavigationRequested(object? sender, string section) =>
        NavigateTo(section);

    private static bool CanNavigateTo(string viewName) => viewName switch
    {
        "Dashboard" => RolePermissions.CanAccessDashboard,
        "Empleados" => RolePermissions.CanAccessEmpleados,
        "Proyectos" => RolePermissions.CanAccessProyectos,
        "Tareas" => RolePermissions.CanAccessTareas,
        "Clientes" => RolePermissions.CanAccessClientes,
        "Reportes" => RolePermissions.CanAccessReportes,
        "Optimizacion" => RolePermissions.CanAccessOptimizacion,
        "Configuracion" => RolePermissions.CanAccessConfiguracion,
        "Perfil" => SessionService.IsAuthenticated,
        _ => false
    };

    private void OnAppNotificationShown(object? sender, string message)
    {
        if (DesktopNotificationService.IsEnabled)
            ShowDesktopToast("WorkFlowDesk", message);
    }

    public void ShowDesktopToast(string title, string message)
    {
        ToastTitle.Text = title;
        ToastMessage.Text = message;
        DesktopToast.Visibility = Visibility.Visible;

        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(220));
        DesktopToast.BeginAnimation(OpacityProperty, fadeIn);

        _toastTimer?.Stop();
        _toastTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4) };
        _toastTimer.Tick += (_, _) =>
        {
            _toastTimer.Stop();
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(220));
            fadeOut.Completed += (_, _) => DesktopToast.Visibility = Visibility.Collapsed;
            DesktopToast.BeginAnimation(OpacityProperty, fadeOut);
        };
        _toastTimer.Start();
    }
}
