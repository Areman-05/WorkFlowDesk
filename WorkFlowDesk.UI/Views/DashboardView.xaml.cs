using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.UI.Services;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class DashboardView : UserControl
{
    private readonly DashboardViewModel _viewModel;
    private readonly IProyectoService _proyectoService;
    private readonly IClienteService _clienteService;
    private readonly IEmpleadoService _empleadoService;

    public DashboardView(
        DashboardViewModel viewModel,
        IProyectoService proyectoService,
        IClienteService clienteService,
        IEmpleadoService empleadoService)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _proyectoService = proyectoService;
        _clienteService = clienteService;
        _empleadoService = empleadoService;
        DataContext = viewModel;
        viewModel.DatosActualizados += OnDatosActualizados;
        viewModel.ProyectoEdicionSolicitado += OnProyectoEdicionSolicitado;
        viewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(DashboardViewModel.IsLoading))
                ActualizarAnimacionIconoRefresco();
        };
    }

    private void OnDatosActualizados(object? sender, EventArgs e) =>
        NotificationService.ShowSuccess("Datos actualizados correctamente.");

    private async void OnProyectoEdicionSolicitado(object? sender, int proyectoId)
    {
        try
        {
            var proyecto = await _proyectoService.GetByIdAsync(proyectoId);
            if (proyecto == null)
            {
                NotificationService.ShowError("No se encontró el proyecto seleccionado.");
                return;
            }

            var formViewModel = new ProyectoFormViewModel(_proyectoService, _clienteService, _empleadoService, proyecto);
            if (DialogService.ShowProyectoForm(formViewModel) == true)
                await _viewModel.CargarEstadisticasCommand.ExecuteAsync(null);
        }
        catch (Exception ex)
        {
            NotificationService.ShowError($"Error al abrir el proyecto: {ex.Message}");
        }
    }

    private void ActualizarAnimacionIconoRefresco()
    {
        if (RefreshIconRotation == null) return;

        if (_viewModel.IsLoading)
        {
            var spin = new DoubleAnimation(0, 360, TimeSpan.FromSeconds(0.8))
            {
                RepeatBehavior = RepeatBehavior.Forever
            };
            RefreshIconRotation.BeginAnimation(RotateTransform.AngleProperty, spin);
        }
        else
        {
            RefreshIconRotation.BeginAnimation(RotateTransform.AngleProperty, null);
            RefreshIconRotation.Angle = 0;
        }
    }
}
