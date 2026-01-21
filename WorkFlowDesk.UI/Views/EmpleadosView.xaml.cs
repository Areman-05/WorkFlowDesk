using System.Windows.Controls;
using WorkFlowDesk.UI.Services;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class EmpleadosView : UserControl
{
    private readonly EmpleadosViewModel _viewModel;

    public EmpleadosView(EmpleadosViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;

        _viewModel.EmpleadoCreado += OnEmpleadoCreado;
        _viewModel.EmpleadoEditado += OnEmpleadoEditado;
    }

    private void OnEmpleadoCreado(object? sender, Domain.Entities.Empleado empleado)
    {
        var empleadoService = ServiceLocator.GetService<Services.Interfaces.IEmpleadoService>();
        var formViewModel = new EmpleadoFormViewModel(empleadoService, empleado);
        
        if (DialogService.ShowEmpleadoForm(formViewModel) == true)
        {
            _viewModel.CargarEmpleadosCommand.ExecuteAsync(null);
        }
    }

    private void OnEmpleadoEditado(object? sender, Domain.Entities.Empleado empleado)
    {
        var empleadoService = ServiceLocator.GetService<Services.Interfaces.IEmpleadoService>();
        var formViewModel = new EmpleadoFormViewModel(empleadoService, empleado);
        
        if (DialogService.ShowEmpleadoForm(formViewModel) == true)
        {
            _viewModel.CargarEmpleadosCommand.ExecuteAsync(null);
        }
    }
}
