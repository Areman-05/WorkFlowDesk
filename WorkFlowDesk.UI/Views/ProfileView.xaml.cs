using System.Windows;
using System.Windows.Controls;
using WorkFlowDesk.UI.Helpers;
using WorkFlowDesk.UI.Services;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class ProfileView : UserControl
{
    public ProfileView(ProfileViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        ViewConfirmationHelper.BindConfirmaciones(viewModel);

        viewModel.OperacionCompletada += (_, mensaje) =>
            NotificationService.ShowSuccess(mensaje);

        viewModel.ExportacionCompletada += (_, path) =>
            NotificationService.ShowSuccess($"Reporte de perfil exportado correctamente.\n{path}");

        viewModel.TemaChanged += (_, tema) => AppThemeService.Apply(tema);

        viewModel.NotificacionesChanged += (_, enabled) =>
        {
            DesktopNotificationService.SetEnabled(enabled);
            if (enabled)
                DesktopNotificationService.Show("WorkFlowDesk", "Notificaciones de escritorio activadas.");
        };

        viewModel.CuentaDesactivada += (_, _) =>
        {
            NotificationService.ShowInfo("Su cuenta ha sido desactivada.");
            AuthFlowService.LogoutAndShowLogin(Window.GetWindow(this));
        };
    }
}
