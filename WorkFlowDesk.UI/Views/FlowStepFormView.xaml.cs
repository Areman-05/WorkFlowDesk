using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class FlowStepFormView : UserControl
{
    public FlowStepFormView()
    {
        InitializeComponent();
    }

    private void OnOverlayClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is FlowStepFormViewModel vm)
            vm.CancelarCommand.Execute(null);
    }

    private void OnModalClick(object sender, MouseButtonEventArgs e) => e.Handled = true;

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Escape || DataContext is not FlowStepFormViewModel vm)
            return;

        vm.CancelarCommand.Execute(null);
    }
}
