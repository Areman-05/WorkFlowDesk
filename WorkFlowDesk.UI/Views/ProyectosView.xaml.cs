using System.Windows.Controls;
using WorkFlowDesk.UI.Services;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class ProyectosView : UserControl
{
    private readonly ProyectosViewModel _viewModel;

    public ProyectosView(ProyectosViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;

        _viewModel.ProyectoCreado += OnProyectoCreado;
        _viewModel.ProyectoEditado += OnProyectoEditado;
    }

    private void OnProyectoCreado(object? sender, Domain.Entities.Proyecto _)
    {
        var proyectoService = ServiceLocator.GetService<Services.Interfaces.IProyectoService>();
        var clienteService = ServiceLocator.GetService<Services.Interfaces.IClienteService>();
        var empleadoService = ServiceLocator.GetService<Services.Interfaces.IEmpleadoService>();
        var formViewModel = new ProyectoFormViewModel(proyectoService, clienteService, empleadoService, proyecto: null);

        if (DialogService.ShowProyectoForm(formViewModel) == true)
        {
            _viewModel.CargarProyectosCommand.ExecuteAsync(null);
        }
    }

    private void OnProyectoEditado(object? sender, Domain.Entities.Proyecto proyecto)
    {
        var proyectoService = ServiceLocator.GetService<Services.Interfaces.IProyectoService>();
        var clienteService = ServiceLocator.GetService<Services.Interfaces.IClienteService>();
        var empleadoService = ServiceLocator.GetService<Services.Interfaces.IEmpleadoService>();
        var formViewModel = new ProyectoFormViewModel(proyectoService, clienteService, empleadoService, proyecto);

        if (DialogService.ShowProyectoForm(formViewModel) == true)
        {
            _viewModel.CargarProyectosCommand.ExecuteAsync(null);
        }
    }
}
