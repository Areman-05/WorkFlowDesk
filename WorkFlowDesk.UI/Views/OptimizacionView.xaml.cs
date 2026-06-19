using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WorkFlowDesk.UI.Helpers;
using WorkFlowDesk.UI.Services;
using WorkFlowDesk.ViewModel.Models;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class OptimizacionView : UserControl
{
    private readonly OptimizacionViewModel _viewModel;

    public OptimizacionView(OptimizacionViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;

        ViewConfirmationHelper.BindConfirmaciones(viewModel);

        viewModel.AccionCompletada += (_, mensaje) =>
            NotificationService.ShowSuccess(mensaje);
        viewModel.ExportacionCompletada += (_, path) =>
            NotificationService.ShowSuccess($"Automatizaciones exportadas correctamente.\n{path}");
        viewModel.EditorPasoSolicitado += OnEditorPasoSolicitado;
    }

    private void OnNestedScrollViewerPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        e.Handled = true;
        MainScrollViewer.ScrollToVerticalOffset(MainScrollViewer.VerticalOffset - e.Delta);
    }

    private void OnAutomatizacionToggleClick(object sender, MouseButtonEventArgs e) =>
        e.Handled = true;

    private void OnEditorPasoSolicitado(object? sender, FlowStepEditorRequest request)
    {
        var formViewModel = new FlowStepFormViewModel(request.PasoExistente, request.TipoPorDefecto);
        FlowStepItem? pasoGuardado = null;
        string? pasoEliminadoId = null;
        var eliminar = false;

        formViewModel.Guardado += (_, paso) =>
        {
            pasoGuardado = paso;
            eliminar = false;
        };
        formViewModel.Eliminado += (_, id) =>
        {
            pasoEliminadoId = id;
            eliminar = true;
        };

        if (DialogService.ShowFlowStepForm(formViewModel) != true)
            return;

        if (eliminar && pasoEliminadoId != null)
            _viewModel.EliminarPaso(pasoEliminadoId);
        else if (pasoGuardado != null)
            _viewModel.AplicarPaso(pasoGuardado, request.EsNuevo);
    }
}
