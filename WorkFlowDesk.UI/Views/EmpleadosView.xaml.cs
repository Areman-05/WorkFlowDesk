using System.Windows.Controls;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class EmpleadosView : UserControl
{
    public EmpleadosView(EmpleadosViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
