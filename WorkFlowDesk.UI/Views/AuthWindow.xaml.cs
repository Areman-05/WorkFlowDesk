using System.ComponentModel;
using System.Windows;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

/// <summary>Ventana única de autenticación (login y registro).</summary>
public partial class AuthWindow : Window
{
    private const double AnchoFijo = 532;
    private const double AltoLogin = 680;
    private const double AltoRegistro = 760;

    private readonly AuthShellViewModel _shell;

    public AuthWindow(AuthShellViewModel shell)
    {
        InitializeComponent();
        _shell = shell;
        DataContext = shell;
        _shell.PropertyChanged += OnShellPropertyChanged;
        AplicarTamanoFijo(_shell.MostrarRegistro);
    }

    private void OnShellPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AuthShellViewModel.MostrarRegistro))
            AplicarTamanoFijo(_shell.MostrarRegistro);
    }

    private void AplicarTamanoFijo(bool registro)
    {
        Width = AnchoFijo;
        MinWidth = AnchoFijo;
        MaxWidth = AnchoFijo;
        Height = registro ? AltoRegistro : AltoLogin;
        MinHeight = Height;
        MaxHeight = Height;
        Title = registro ? "Crear cuenta - WorkFlowDesk" : "Iniciar sesión - WorkFlowDesk";
    }
}
