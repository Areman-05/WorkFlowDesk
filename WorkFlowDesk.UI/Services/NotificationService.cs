using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WorkFlowDesk.UI.Services;

/// <summary>Muestra notificaciones (éxito, error, advertencia, confirmación).</summary>
public static class NotificationService
{
    public static event EventHandler<string>? NotificationShown;

    /// <summary>Muestra un mensaje de éxito (icono de información).</summary>
    public static void ShowSuccess(string message, string title = "Éxito")
    {
        ShowNotification(message, title, MessageBoxImage.Information);
        NotificationShown?.Invoke(null, message);
    }

    /// <summary>Muestra un mensaje de error (icono de error).</summary>
    public static void ShowError(string message, string title = "Error")
    {
        ShowNotification(message, title, MessageBoxImage.Error);
        NotificationShown?.Invoke(null, message);
    }

    /// <summary>Muestra un mensaje de advertencia.</summary>
    public static void ShowWarning(string message, string title = "Advertencia")
    {
        ShowNotification(message, title, MessageBoxImage.Warning);
        NotificationShown?.Invoke(null, message);
    }

    public static void ShowInfo(string message, string title = "Información")
    {
        ShowNotification(message, title, MessageBoxImage.Information);
        NotificationShown?.Invoke(null, message);
    }

    public static MessageBoxResult ShowConfirmation(string message, string title = "Confirmar")
    {
        return MessageBox.Show(
            message,
            title,
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);
    }

    private static void ShowNotification(string message, string title, MessageBoxImage icon)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, icon);
    }
}
