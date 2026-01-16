using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.ViewModel.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private readonly IAuthenticationService _authenticationService;
    private string _nombreUsuario = string.Empty;
    private string _password = string.Empty;

    public LoginViewModel(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
        LoginCommand = new AsyncRelayCommand(LoginAsync, CanLogin);
    }

    public string NombreUsuario
    {
        get => _nombreUsuario;
        set
        {
            SetProperty(ref _nombreUsuario, value);
            LoginCommand.NotifyCanExecuteChanged();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            SetProperty(ref _password, value);
            LoginCommand.NotifyCanExecuteChanged();
        }
    }

    public IAsyncRelayCommand LoginCommand { get; }

    public event EventHandler<Usuario>? LoginExitoso;

    private bool CanLogin() => !string.IsNullOrWhiteSpace(NombreUsuario) && !string.IsNullOrWhiteSpace(Password);

    private async Task LoginAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var usuario = await _authenticationService.AutenticarAsync(NombreUsuario, Password);
            if (usuario != null)
            {
                LoginExitoso?.Invoke(this, usuario);
            }
            else
            {
                ErrorMessage = "Usuario o contraseña incorrectos";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al iniciar sesión: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
