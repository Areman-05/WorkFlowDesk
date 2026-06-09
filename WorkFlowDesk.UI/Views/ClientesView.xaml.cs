using System.Windows.Controls;
using WorkFlowDesk.UI.Helpers;
using WorkFlowDesk.UI.Services;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class ClientesView : UserControl
{
    private readonly ClientesViewModel _viewModel;

    public ClientesView(ClientesViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;
        ViewConfirmationHelper.BindConfirmaciones(viewModel);

        _viewModel.ClienteCreado += OnClienteCreado;
        _viewModel.ClienteEditado += OnClienteEditado;
        _viewModel.ExportacionCompletada += OnExportacionCompletada;
    }

    private void OnExportacionCompletada(object? sender, string path)
    {
        NotificationService.ShowSuccess($"Datos exportados correctamente.\n{path}");
    }

    private void OnClienteCreado(object? sender, Domain.Entities.Cliente cliente)
    {
        var clienteService = ServiceLocator.GetService<IClienteService>();
        var formViewModel = new ClienteFormViewModel(clienteService, cliente);
        
        if (DialogService.ShowClienteForm(formViewModel) == true)
        {
            _viewModel.CargarClientesCommand.ExecuteAsync(null);
        }
    }

    private void OnClienteEditado(object? sender, Domain.Entities.Cliente cliente)
    {
        var clienteService = ServiceLocator.GetService<IClienteService>();
        var formViewModel = new ClienteFormViewModel(clienteService, cliente);
        
        if (DialogService.ShowClienteForm(formViewModel) == true)
        {
            _viewModel.CargarClientesCommand.ExecuteAsync(null);
        }
    }
}
