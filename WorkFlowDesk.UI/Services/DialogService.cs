using System.Windows;
using System.Windows.Input;
using WorkFlowDesk.UI.Views;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Services;

/// <summary>Muestra formularios modales (Empleado, Cliente, Proyecto, Tarea).</summary>
public static class DialogService
{
    /// <summary>Muestra el formulario de empleado (alta/edición) en un diálogo modal.</summary>
    public static bool? ShowEmpleadoForm(EmpleadoFormViewModel viewModel)
    {
        var owner = Application.Current.MainWindow;
        var window = new Window
        {
            Title = viewModel.Titulo,
            WindowStyle = WindowStyle.None,
            AllowsTransparency = true,
            Background = System.Windows.Media.Brushes.Transparent,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = owner,
            ResizeMode = ResizeMode.NoResize,
            ShowInTaskbar = false,
            Width = owner?.ActualWidth > 0 ? owner.ActualWidth : 1200,
            Height = owner?.ActualHeight > 0 ? owner.ActualHeight : 720,
            Left = owner?.Left ?? 0,
            Top = owner?.Top ?? 0
        };

        var formView = new EmpleadoFormView();
        formView.DataContext = viewModel;

        viewModel.Guardado += (_, _) => window.DialogResult = true;
        viewModel.Cancelado += (_, _) => window.DialogResult = false;

        window.KeyDown += (_, e) =>
        {
            if (e.Key == Key.Escape)
            {
                viewModel.CancelarCommand.Execute(null);
                window.DialogResult = false;
            }
        };

        window.Content = formView;
        return window.ShowDialog();
    }

    /// <summary>Muestra el formulario de proyecto (alta/edición) en un diálogo modal.</summary>
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

    /// <summary>Muestra el formulario de tarea (alta/edición) en un diálogo modal.</summary>
    public static bool? ShowTareaForm(TareaFormViewModel viewModel)
    {
        var window = new Window
        {
            Title = viewModel.Titulo,
            Width = 720,
            Height = 720,
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

    /// <summary>Muestra el formulario de cliente (alta/edición) en un diálogo modal.</summary>
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
