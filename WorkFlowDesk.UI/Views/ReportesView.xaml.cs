using System.Windows.Controls;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class ReportesView : UserControl
{
    public ReportesView(ReportesViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
