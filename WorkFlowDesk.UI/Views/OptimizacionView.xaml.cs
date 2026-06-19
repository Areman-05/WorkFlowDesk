using System.Windows.Controls;
using WorkFlowDesk.UI.Services;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class OptimizacionView : UserControl
{
    public OptimizacionView(OptimizacionViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.AccionCompletada += (_, mensaje) =>
            NotificationService.ShowSuccess(mensaje);
    }
}
