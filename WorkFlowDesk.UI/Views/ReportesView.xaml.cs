using System.Windows.Controls;
using WorkFlowDesk.UI.Services;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class ReportesView : UserControl
{
    public ReportesView(ReportesViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.ExportacionCompletada += OnExportacionCompletada;
    }

    private void OnExportacionCompletada(object? sender, string path)
    {
        NotificationService.ShowSuccess($"Reporte exportado correctamente.\n{path}");
    }
}
