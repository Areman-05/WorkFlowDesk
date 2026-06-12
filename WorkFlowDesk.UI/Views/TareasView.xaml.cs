using System.Windows.Controls;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.UI.Helpers;
using WorkFlowDesk.UI.Services;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class TareasView : UserControl
{
    private readonly TareasViewModel _viewModel;
    private readonly ITareaService _tareaService;
    private readonly IProyectoService _proyectoService;
    private readonly IEmpleadoService _empleadoService;

    public TareasView(
        TareasViewModel viewModel,
        ITareaService tareaService,
        IProyectoService proyectoService,
        IEmpleadoService empleadoService)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;
        _tareaService = tareaService;
        _proyectoService = proyectoService;
        _empleadoService = empleadoService;
        ViewConfirmationHelper.BindConfirmaciones(viewModel);

        _viewModel.TareaCreada += OnTareaCreada;
        _viewModel.TareaEditada += OnTareaEditada;
        _viewModel.ExportacionCompletada += OnExportacionCompletada;
    }

    private void OnExportacionCompletada(object? sender, string path)
    {
        NotificationService.ShowSuccess($"Datos exportados correctamente.\n{path}");
    }

    private void OnTareaCreada(object? sender, Domain.Entities.Tarea _)
    {
        var formViewModel = new TareaFormViewModel(_tareaService, _proyectoService, _empleadoService, tarea: null);

        if (DialogService.ShowTareaForm(formViewModel) == true)
        {
            _viewModel.CargarTareasCommand.ExecuteAsync(null);
        }
    }

    private void OnTareaEditada(object? sender, Domain.Entities.Tarea tarea)
    {
        var formViewModel = new TareaFormViewModel(_tareaService, _proyectoService, _empleadoService, tarea);

        if (DialogService.ShowTareaForm(formViewModel) == true)
        {
            _viewModel.CargarTareasCommand.ExecuteAsync(null);
        }
    }
}
