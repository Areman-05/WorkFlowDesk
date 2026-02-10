using System.Windows;

namespace WorkFlowDesk.UI.Services;

/// <summary>Muestra cuadros de diálogo (confirmación, error, información).</summary>
public static class MessageBoxService
{
    /// <summary>Muestra un cuadro de confirmación Sí/No y devuelve la respuesta.</summary>
    public static MessageBoxResult ShowConfirmation(string message, string title = "Confirmar")
    {
        return MessageBox.Show(
            message,
            title,
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);
    }

    /// <summary>Muestra un mensaje de error (icono de error).</summary>
    public static void ShowError(string message, string title = "Error")
    {
        MessageBox.Show(
            message,
            title,
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }

    /// <summary>Muestra un mensaje informativo.</summary>
    public static void ShowInfo(string message, string title = "Información")
    {
        MessageBox.Show(
            message,
            title,
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    /// <summary>Muestra un mensaje de advertencia.</summary>
    public static void ShowWarning(string message, string title = "Advertencia")
    {
        MessageBox.Show(
            message,
            title,
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }
}
