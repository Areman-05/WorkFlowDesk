using System.Windows.Controls;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class TareasView : UserControl
{
    public TareasView(TareasViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
