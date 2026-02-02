using System.Windows.Controls;
using WorkFlowDesk.Services.Interfaces;
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

        _viewModel.ClienteCreado += OnClienteCreado;
        _viewModel.ClienteEditado += OnClienteEditado;
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
