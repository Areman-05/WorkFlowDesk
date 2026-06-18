using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WorkFlowDesk.UI.Controls;
using WorkFlowDesk.UI.Helpers;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class EmpleadoFormView : UserControl
{
    public EmpleadoFormView()
    {
        InitializeComponent();
        Focusable = true;
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        Focus();
        await AvatarImageLoader.PreloadCatalogAsync();
        RefreshAvatarImages(this);
    }

    private static void RefreshAvatarImages(DependencyObject parent)
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is AvatarImage avatar)
                avatar.Refresh();
            else
                RefreshAvatarImages(child);
        }
    }

    private void OnOverlayClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is EmpleadoFormViewModel vm)
            vm.CancelarCommand.Execute(null);
    }

    private void OnModalClick(object sender, MouseButtonEventArgs e) => e.Handled = true;

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Escape || DataContext is not EmpleadoFormViewModel vm)
            return;

        vm.CancelarCommand.Execute(null);
        e.Handled = true;
    }
}
