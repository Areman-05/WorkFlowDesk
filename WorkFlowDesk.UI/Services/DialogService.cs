using System.Windows;
using WorkFlowDesk.UI.Views;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Services;

/// <summary>Muestra formularios modales (Empleado, Cliente, Proyecto, Tarea).</summary>
public static class DialogService
{
    /// <summary>Muestra el formulario de empleado (alta/edición) en un diálogo modal.</summary>
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

        var formView = new ProyectoFormView();
        formView.DataContext = viewModel;

        viewModel.Guardado += (s, e) => window.DialogResult = true;
        viewModel.Cancelado += (s, e) => window.DialogResult = false;

        window.Content = formView;
        return window.ShowDialog();
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

        var formView = new TareaFormView();
        formView.DataContext = viewModel;

        viewModel.Guardado += (s, e) => window.DialogResult = true;
        viewModel.Cancelado += (s, e) => window.DialogResult = false;

        window.Content = formView;
        return window.ShowDialog();
    }

    public static bool? ShowClienteForm(ClienteFormViewModel viewModel)
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

        var formView = new ClienteFormView();
        formView.DataContext = viewModel;

        viewModel.Guardado += (s, e) => window.DialogResult = true;
        viewModel.Cancelado += (s, e) => window.DialogResult = false;

        window.Content = formView;
        return window.ShowDialog();
    }
}
