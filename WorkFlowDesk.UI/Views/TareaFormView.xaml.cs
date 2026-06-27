using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class TareaFormView : UserControl
{
    public TareaFormView()
    {
        InitializeComponent();
        Focusable = true;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Focus();
        if (DataContext is TareaFormViewModel vm)
        {
            vm.SolicitarAdjunto += OnSolicitarAdjunto;
            vm.AbrirArchivoSolicitado += OnAbrirArchivo;
        }
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
        if (!File.Exists(ruta)) return;
        Process.Start(new ProcessStartInfo(ruta) { UseShellExecute = true });
    }

    private void OnOverlayClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is TareaFormViewModel vm)
            vm.CancelarCommand.Execute(null);
    }

    private void OnModalClick(object sender, MouseButtonEventArgs e) => e.Handled = true;

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Escape || DataContext is not TareaFormViewModel vm)
            return;

        vm.CancelarCommand.Execute(null);
        e.Handled = true;
    }
}
