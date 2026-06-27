using System.Diagnostics;
using System.Windows.Controls;
using Microsoft.Win32;
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
    private readonly ITareaExtensionService _extensionService;
    private readonly IAttachmentService _attachmentService;
    private readonly IActivityLogService _activityLogService;

    public TareasView(
        TareasViewModel viewModel,
        ITareaService tareaService,
        IProyectoService proyectoService,
        IEmpleadoService empleadoService,
        ITareaExtensionService extensionService,
        IAttachmentService attachmentService,
        IActivityLogService activityLogService)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;
        _tareaService = tareaService;
        _proyectoService = proyectoService;
        _empleadoService = empleadoService;
        _extensionService = extensionService;
        _attachmentService = attachmentService;
        _activityLogService = activityLogService;
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
        ShowForm(tarea: null);
    }

    private void OnTareaEditada(object? sender, Domain.Entities.Tarea tarea)
    {
        ShowForm(tarea);
    }

    private void ShowForm(Domain.Entities.Tarea? tarea)
    {
        var formViewModel = new TareaFormViewModel(
            _tareaService,
            _proyectoService,
            _empleadoService,
            _extensionService,
            _attachmentService,
            _activityLogService,
            tarea);

        formViewModel.SolicitarAdjunto += OnSolicitarAdjunto;
        formViewModel.AbrirArchivoSolicitado += OnAbrirArchivo;

        if (DialogService.ShowTareaForm(formViewModel) == true)
            _viewModel.CargarTareasCommand.ExecuteAsync(null);
    }

    private async void OnSolicitarAdjunto(object? sender, EventArgs e)
    {
        if (sender is not TareaFormViewModel vm) return;

        var dialog = new OpenFileDialog
        {
            Title = "Seleccionar archivo",
            Filter = "Todos los archivos (*.*)|*.*"
        };

        if (dialog.ShowDialog() == true)
            await vm.AgregarAdjuntoDesdeRutaAsync(dialog.FileName);
    }

    private static void OnAbrirArchivo(object? sender, string ruta)
    {
        if (!System.IO.File.Exists(ruta)) return;
        Process.Start(new ProcessStartInfo(ruta) { UseShellExecute = true });
    }
}
