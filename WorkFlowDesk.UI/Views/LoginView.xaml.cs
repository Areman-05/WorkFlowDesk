using System.Windows;
using System.Windows.Controls;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

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
}
