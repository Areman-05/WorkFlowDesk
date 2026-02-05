using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>ViewModel de la pantalla de inicio de sesión.</summary>
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

    /// <summary>Asigna la contraseña desde la vista (PasswordBox no permite binding directo).</summary>
    public void SetPasswordFromView(string password)
    {
        Password = password ?? string.Empty;
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
            ExceptionHandler.LogException(ex);
            ErrorMessage = ExceptionHandler.HandleException(ex);
        }
        finally
        {
            IsLoading = false;
        }
    }
}
