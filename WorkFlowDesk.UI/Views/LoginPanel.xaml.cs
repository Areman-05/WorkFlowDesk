using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class LoginPanel : UserControl
{
    public LoginPanel()
    {
        InitializeComponent();
    }

    private async void OnLoginClick(object sender, RoutedEventArgs e) => await AttemptLoginAsync();

    private void OnLoginFieldKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            _ = AttemptLoginAsync();
            e.Handled = true;
        }
    }

    private async Task AttemptLoginAsync()
    {
        if (DataContext is not LoginViewModel vm)
            return;

        vm.NombreUsuario = NombreUsuarioBox.Text.Trim();
        vm.SetPasswordFromView(PasswordRevealBox.Password);
        vm.LoginCommand.NotifyCanExecuteChanged();

        await vm.LoginCommand.ExecuteAsync(null);
    }
}
