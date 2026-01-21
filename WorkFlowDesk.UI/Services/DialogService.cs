using System.Windows;
using WorkFlowDesk.UI.Views;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Services;

public static class DialogService
{
    public static bool? ShowEmpleadoForm(EmpleadoFormViewModel viewModel)
    {
        var window = new Window
        {
            Title = viewModel.Titulo,
            Width = 600,
            Height = 600,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = Application.Current.MainWindow,
            ResizeMode = ResizeMode.CanResize
        };

        var formView = new EmpleadoFormView();
        formView.DataContext = viewModel;

        viewModel.Guardado += (s, e) => window.DialogResult = true;
        viewModel.Cancelado += (s, e) => window.DialogResult = false;

        window.Content = formView;
        return window.ShowDialog();
    }

    public static bool? ShowProyectoForm(ProyectoFormViewModel viewModel)
    {
        var window = new Window
        {
            Title = viewModel.Titulo,
            Width = 700,
            Height = 600,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = Application.Current.MainWindow,
            ResizeMode = ResizeMode.CanResize
        };

        // Por ahora retornamos null, se implementará cuando tengamos la vista
        return null;
    }

    public static bool? ShowTareaForm(TareaFormViewModel viewModel)
    {
        var window = new Window
        {
            Title = viewModel.Titulo,
            Width = 700,
            Height = 600,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = Application.Current.MainWindow,
            ResizeMode = ResizeMode.CanResize
        };

        // Por ahora retornamos null, se implementará cuando tengamos la vista
        return null;
    }
}
