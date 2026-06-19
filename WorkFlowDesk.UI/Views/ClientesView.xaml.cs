using System.Windows;
using System.Windows.Controls;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.UI.Helpers;
using WorkFlowDesk.UI.Services;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class ClientesView : UserControl
{
    private readonly ClientesViewModel _viewModel;
    private readonly IClienteService _clienteService;

    public ClientesView(ClientesViewModel viewModel, IClienteService clienteService)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;
        _clienteService = clienteService;
        ViewConfirmationHelper.BindConfirmaciones(viewModel);

        _viewModel.ClienteCreado += OnClienteCreado;
        _viewModel.ClienteEditado += OnClienteEditado;
        _viewModel.ExportacionCompletada += OnExportacionCompletada;
        _viewModel.MensajePreparado += OnMensajePreparado;
    }

    private void OnMensajePreparado(object? sender, string mensaje)
    {
        Clipboard.SetText(mensaje);
        NotificationService.ShowSuccess("Mensaje copiado al portapapeles. Pégalo en tu cliente de correo.");
    }

    private void OnExportacionCompletada(object? sender, string path)
    {
        NotificationService.ShowSuccess($"Datos exportados correctamente.\n{path}");
    }

    private void OnClienteCreado(object? sender, Domain.Entities.Cliente cliente)
    {
        var formViewModel = new ClienteFormViewModel(_clienteService, cliente);

        if (DialogService.ShowClienteForm(formViewModel) == true)
        {
            _viewModel.CargarClientesCommand.ExecuteAsync(null);
        }
    }

    private void OnClienteEditado(object? sender, Domain.Entities.Cliente cliente)
    {
        var formViewModel = new ClienteFormViewModel(_clienteService, cliente);

        if (DialogService.ShowClienteForm(formViewModel) == true)
        {
            _viewModel.CargarClientesCommand.ExecuteAsync(null);
        }
    }
}
