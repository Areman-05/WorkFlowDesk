using System.Windows;
using System.Windows.Controls;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class LoginPanel : UserControl
{
    public LoginPanel()
    {
        InitializeComponent();
    }

    private void OnLoginClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel vm)
        {
            vm.SetPasswordFromView(PasswordBox.Password);
            vm.LoginCommand.Execute(null);
        }
    }
}
