using System.Windows.Controls;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class ClientesView : UserControl
{
    public ClientesView(ClientesViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
