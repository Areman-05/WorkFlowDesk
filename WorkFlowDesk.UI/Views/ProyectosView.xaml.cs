using System.Windows.Controls;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class ProyectosView : UserControl
{
    public ProyectosView(ProyectosViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
