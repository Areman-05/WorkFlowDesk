using System.Windows.Controls;
using WorkFlowDesk.UI.Helpers;
using WorkFlowDesk.UI.Services;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class ConfiguracionView : UserControl
{
    public ConfiguracionView(ConfiguracionViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        ViewConfirmationHelper.BindConfirmaciones(viewModel);
        viewModel.OperacionCompletada += (_, mensaje) =>
            NotificationService.ShowSuccess(mensaje);
    }
}
