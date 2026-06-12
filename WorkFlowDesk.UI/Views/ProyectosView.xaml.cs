using System.Windows.Controls;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.UI.Helpers;
using WorkFlowDesk.UI.Services;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class ProyectosView : UserControl
{
    private readonly ProyectosViewModel _viewModel;
    private readonly IProyectoService _proyectoService;
    private readonly IClienteService _clienteService;
    private readonly IEmpleadoService _empleadoService;

    public ProyectosView(
        ProyectosViewModel viewModel,
        IProyectoService proyectoService,
        IClienteService clienteService,
        IEmpleadoService empleadoService)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;
        _proyectoService = proyectoService;
        _clienteService = clienteService;
        _empleadoService = empleadoService;
        ViewConfirmationHelper.BindConfirmaciones(viewModel);

        _viewModel.ProyectoCreado += OnProyectoCreado;
        _viewModel.ProyectoEditado += OnProyectoEditado;
        _viewModel.ExportacionCompletada += OnExportacionCompletada;
    }

    private void OnExportacionCompletada(object? sender, string path)
    {
        NotificationService.ShowSuccess($"Datos exportados correctamente.\n{path}");
    }

    private void OnProyectoCreado(object? sender, Domain.Entities.Proyecto _)
    {
        var formViewModel = new ProyectoFormViewModel(_proyectoService, _clienteService, _empleadoService, proyecto: null);

        if (DialogService.ShowProyectoForm(formViewModel) == true)
        {
            _viewModel.CargarProyectosCommand.ExecuteAsync(null);
        }
    }

    private void OnProyectoEditado(object? sender, Domain.Entities.Proyecto proyecto)
    {
        var formViewModel = new ProyectoFormViewModel(_proyectoService, _clienteService, _empleadoService, proyecto);

        if (DialogService.ShowProyectoForm(formViewModel) == true)
        {
            _viewModel.CargarProyectosCommand.ExecuteAsync(null);
        }
    }
}
