using System.Windows;
using System.Windows.Media.Animation;

namespace WorkFlowDesk.UI.Services;

/// <summary>Notificaciones visuales en la ventana principal (esquina inferior derecha).</summary>
public static class DesktopNotificationService
{
    public static bool IsEnabled { get; private set; } = true;

    public static void SetEnabled(bool enabled) => IsEnabled = enabled;

    public static void Show(string title, string message)
    {
        if (!IsEnabled)
            return;

        Application.Current?.Dispatcher.Invoke(() =>
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
                mainWindow.ShowDesktopToast(title, message);
            else
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        });
    }

    public static void Dispose()
    {
    }
}
