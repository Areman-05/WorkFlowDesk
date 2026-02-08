using System.Windows;
using System.Windows.Controls;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

/// <summary>Ventana de inicio de sesión (usuario y contraseña).</summary>
public partial class LoginView : Window
{
    public LoginView(LoginViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        
        viewModel.LoginExitoso += (sender, usuario) =>
        {
            DialogResult = true;
            Close();
        };
    }

    /// <summary>Pasa la contraseña del PasswordBox al ViewModel y ejecuta el login (el binding no siempre actualiza a tiempo).</summary>
    private void OnLoginClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel vm)
        {
            vm.SetPasswordFromView(PasswordBox.Password);
            vm.LoginCommand.Execute(null);
        }
    }
}
