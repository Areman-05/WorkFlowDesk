using System.Windows;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

/// <summary>Ventana de registro de nuevo usuario.</summary>
public partial class RegisterView : Window
{
    public RegisterView(RegisterViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        viewModel.RegistroExitoso += (_, _) =>
        {
            DialogResult = true;
            Close();
        };

        viewModel.CancelarRequested += (_, _) =>
        {
            DialogResult = false;
            Close();
        };
    }

    private void OnRegisterClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is RegisterViewModel vm)
        {
            vm.SetPasswordsFromView(PasswordBox.Password, ConfirmPasswordBox.Password);
            vm.RegisterCommand.Execute(null);
        }
    }
}
