using System.ComponentModel;
using System.Windows;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

/// <summary>Ventana única de autenticación (login y registro).</summary>
public partial class AuthWindow : Window
{
    private readonly AuthShellViewModel _shell;

    public AuthWindow(AuthShellViewModel shell)
    {
        InitializeComponent();
        _shell = shell;
        DataContext = shell;
        _shell.PropertyChanged += OnShellPropertyChanged;
        ActualizarTitulo(_shell.MostrarRegistro);
    }

    private void OnShellPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AuthShellViewModel.MostrarRegistro))
            ActualizarTitulo(_shell.MostrarRegistro);
    }

    private void ActualizarTitulo(bool registro)
    {
        Title = registro ? "Crear cuenta - WorkFlowDesk" : "Iniciar sesión - WorkFlowDesk";
    }
}
