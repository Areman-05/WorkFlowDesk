using System.Windows;

namespace WorkFlowDesk.UI.Services;

public static class MessageBoxService
{
    public static MessageBoxResult ShowConfirmation(string message, string title = "Confirmar")
    {
        return MessageBox.Show(
            message,
            title,
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);
    }

    public static void ShowError(string message, string title = "Error")
    {
        MessageBox.Show(
            message,
            title,
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }

    public static void ShowInfo(string message, string title = "Información")
    {
        MessageBox.Show(
            message,
            title,
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    public static void ShowWarning(string message, string title = "Advertencia")
    {
        MessageBox.Show(
            message,
            title,
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }
}
