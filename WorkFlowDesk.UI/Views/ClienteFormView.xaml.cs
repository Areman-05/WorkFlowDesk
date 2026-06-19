using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class ClienteFormView : UserControl
{
    public ClienteFormView()
    {
        InitializeComponent();
        Focusable = true;
        Loaded += (_, _) => Focus();
    }

    private void OnOverlayClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is ClienteFormViewModel vm)
            vm.CancelarCommand.Execute(null);
    }

    private void OnModalClick(object sender, MouseButtonEventArgs e) => e.Handled = true;

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Escape || DataContext is not ClienteFormViewModel vm)
            return;

        vm.CancelarCommand.Execute(null);
        e.Handled = true;
    }
}
