using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WorkFlowDesk.UI.Services;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class DashboardView : UserControl
{
    private readonly DashboardViewModel _viewModel;

    public DashboardView(DashboardViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        viewModel.DatosActualizados += OnDatosActualizados;
        viewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(DashboardViewModel.IsLoading))
                ActualizarAnimacionIconoRefresco();
        };
    }

    private void OnDatosActualizados(object? sender, EventArgs e) =>
        NotificationService.ShowSuccess("Datos actualizados correctamente.");

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
