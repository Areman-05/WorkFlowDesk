using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WorkFlowDesk.UI.Services;

public static class NotificationService
{
    public static void ShowSuccess(string message, string title = "Éxito")
    {
        ShowNotification(message, title, Brushes.Green);
    }

    public static void ShowError(string message, string title = "Error")
    {
        ShowNotification(message, title, Brushes.Red);
    }

    public static void ShowWarning(string message, string title = "Advertencia")
    {
        ShowNotification(message, title, Brushes.Orange);
    }

    public static void ShowInfo(string message, string title = "Información")
    {
        ShowNotification(message, title, Brushes.Blue);
    }

    private static void ShowNotification(string message, string title, Brush color)
    {
        var window = Application.Current.MainWindow;
        if (window == null) return;

        var notification = new Border
        {
            Background = color,
            CornerRadius = new CornerRadius(5),
            Padding = new Thickness(15, 10, 15, 10),
            Margin = new Thickness(10),
            Opacity = 0.9
        };

        var stackPanel = new StackPanel();
        
        var titleBlock = new TextBlock
        {
            Text = title,
            FontWeight = FontWeights.Bold,
            Foreground = Brushes.White,
            FontSize = 14,
            Margin = new Thickness(0, 0, 0, 5)
        };

        var messageBlock = new TextBlock
        {
            Text = message,
            Foreground = Brushes.White,
            FontSize = 12,
            TextWrapping = TextWrapping.Wrap
        };

        stackPanel.Children.Add(titleBlock);
        stackPanel.Children.Add(messageBlock);
        notification.Child = stackPanel;

        // Por ahora solo mostramos un MessageBox, se puede mejorar con un sistema de notificaciones más avanzado
        MessageBox.Show(message, title, MessageBoxButton.OK, 
            color == Brushes.Red ? MessageBoxImage.Error :
            color == Brushes.Orange ? MessageBoxImage.Warning :
            color == Brushes.Green ? MessageBoxImage.Information :
            MessageBoxImage.Information);
    }
}
