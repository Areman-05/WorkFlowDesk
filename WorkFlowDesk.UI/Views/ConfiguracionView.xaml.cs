using System.Windows.Controls;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class ConfiguracionView : UserControl
{
    public ConfiguracionView(ConfiguracionViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
