using System.Windows.Controls;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class DashboardView : UserControl
{
    public DashboardView(DashboardViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
