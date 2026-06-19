using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using WorkFlowDesk.UI.Services;
using WorkFlowDesk.ViewModel.Models;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class ReportesView : UserControl
{
    private readonly ReportesViewModel _viewModel;

    public ReportesView(ReportesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        viewModel.ExportacionCompletada += OnExportacionCompletada;
        viewModel.DatosActualizados += OnDatosActualizados;
        viewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ReportesViewModel.IsLoading))
                ActualizarAnimacionIconoRefresco();
            else if (e.PropertyName == nameof(ReportesViewModel.ProductividadVisible))
                Dispatcher.BeginInvoke(AnimarBarrasProductividad, DispatcherPriority.Loaded);
        };
    }

    private void OnExportacionCompletada(object? sender, string path)
    {
        NotificationService.ShowSuccess($"Reporte exportado correctamente.\n{path}");
    }

    private void OnDatosActualizados(object? sender, EventArgs e)
    {
        NotificationService.ShowSuccess("Datos actualizados correctamente.");
        Dispatcher.BeginInvoke(AnimarBarrasProductividad, DispatcherPriority.Loaded);
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

    private void AnimarBarrasProductividad()
    {
        ProductividadChart.UpdateLayout();

        var index = 0;
        foreach (var bar in FindVisualChildren<Border>(ProductividadChart))
        {
            if (bar.Tag as string != "BarFill" || bar.DataContext is not ActividadDiaItem item)
                continue;

            BindingOperations.ClearBinding(bar, FrameworkElement.HeightProperty);
            bar.Height = 0;

            var anim = new DoubleAnimation(0, item.AlturaPixelesReporte, TimeSpan.FromMilliseconds(700))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
                BeginTime = TimeSpan.FromMilliseconds(80 * index++)
            };
            bar.BeginAnimation(FrameworkElement.HeightProperty, anim);
        }
    }

    private static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T match)
                yield return match;

            foreach (var descendant in FindVisualChildren<T>(child))
                yield return descendant;
        }
    }
}
