using System.Windows.Controls;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.UI.Helpers;
using WorkFlowDesk.UI.Services;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class EmpleadosView : UserControl
{
    private readonly EmpleadosViewModel _viewModel;
    private readonly IEmpleadoService _empleadoService;

    public EmpleadosView(EmpleadosViewModel viewModel, IEmpleadoService empleadoService)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;
        _empleadoService = empleadoService;
        ViewConfirmationHelper.BindConfirmaciones(viewModel);

        _viewModel.EmpleadoCreado += OnEmpleadoCreado;
        _viewModel.EmpleadoEditado += OnEmpleadoEditado;
        _viewModel.ExportacionCompletada += OnExportacionCompletada;
    }

    private void OnExportacionCompletada(object? sender, string path)
    {
        NotificationService.ShowSuccess($"Datos exportados correctamente.\n{path}");
    }

    private void OnEmpleadoCreado(object? sender, Domain.Entities.Empleado empleado)
    {
        var formViewModel = new EmpleadoFormViewModel(_empleadoService, empleado);

        if (DialogService.ShowEmpleadoForm(formViewModel) == true)
        {
            _viewModel.CargarEmpleadosCommand.ExecuteAsync(null);
        }
    }

    private void OnEmpleadoEditado(object? sender, Domain.Entities.Empleado empleado)
    {
        var formViewModel = new EmpleadoFormViewModel(_empleadoService, empleado);

        if (DialogService.ShowEmpleadoForm(formViewModel) == true)
        {
            _viewModel.CargarEmpleadosCommand.ExecuteAsync(null);
        }
    }
}
