using System.Windows;
using System.Windows.Controls;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class RegisterPanel : UserControl
{
    public RegisterPanel()
    {
        InitializeComponent();
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
