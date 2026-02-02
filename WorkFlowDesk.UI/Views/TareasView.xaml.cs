using System.Windows.Controls;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.UI.Services;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class TareasView : UserControl
{
    private readonly TareasViewModel _viewModel;

    public TareasView(TareasViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;

        _viewModel.TareaCreada += OnTareaCreada;
        _viewModel.TareaEditada += OnTareaEditada;
    }

    private void OnTareaCreada(object? sender, Domain.Entities.Tarea _)
    {
        var tareaService = ServiceLocator.GetService<ITareaService>();
        var proyectoService = ServiceLocator.GetService<IProyectoService>();
        var empleadoService = ServiceLocator.GetService<IEmpleadoService>();
        var formViewModel = new TareaFormViewModel(tareaService, proyectoService, empleadoService, tarea: null);

        if (DialogService.ShowTareaForm(formViewModel) == true)
        {
            _viewModel.CargarTareasCommand.ExecuteAsync(null);
        }
    }

    private void OnTareaEditada(object? sender, Domain.Entities.Tarea tarea)
    {
        var tareaService = ServiceLocator.GetService<ITareaService>();
        var proyectoService = ServiceLocator.GetService<IProyectoService>();
        var empleadoService = ServiceLocator.GetService<IEmpleadoService>();
        var formViewModel = new TareaFormViewModel(tareaService, proyectoService, empleadoService, tarea);

        if (DialogService.ShowTareaForm(formViewModel) == true)
        {
            _viewModel.CargarTareasCommand.ExecuteAsync(null);
        }
    }
}
