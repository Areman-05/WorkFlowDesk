using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Helpers;
using WorkFlowDesk.Common.Services;
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
    private string _pinSecundario = string.Empty;
    private Usuario? _pendingUsuario;
    private bool _requiresPinVerification;

    public LoginViewModel(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
        LoginCommand = new AsyncRelayCommand(LoginAsync, CanLogin);
        VerifyPinCommand = new RelayCommand(VerifyPin, CanVerifyPin);
        VolverAlLoginCommand = new RelayCommand(VolverAlLogin);
        AbrirRegistroCommand = new RelayCommand(() => AbrirRegistroRequested?.Invoke(this, EventArgs.Empty));
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

    public string PinSecundario
    {
        get => _pinSecundario;
        set
        {
            SetProperty(ref _pinSecundario, value);
            VerifyPinCommand.NotifyCanExecuteChanged();
        }
    }

    public bool RequiresPinVerification
    {
        get => _requiresPinVerification;
        private set => SetProperty(ref _requiresPinVerification, value);
    }

    /// <summary>Asigna la contraseña desde la vista (PasswordBox no permite binding directo).</summary>
    public void SetPasswordFromView(string password) => Password = password ?? string.Empty;

    /// <summary>Asigna el PIN desde la vista.</summary>
    public void SetPinFromView(string pin) => PinSecundario = pin ?? string.Empty;

    public IAsyncRelayCommand LoginCommand { get; }
    public IRelayCommand VerifyPinCommand { get; }
    public IRelayCommand VolverAlLoginCommand { get; }
    public IRelayCommand AbrirRegistroCommand { get; }

    public event EventHandler<Usuario>? LoginExitoso;
    public event EventHandler? AbrirRegistroRequested;

    private bool CanLogin() =>
        !RequiresPinVerification &&
        !string.IsNullOrWhiteSpace(NombreUsuario) &&
        !string.IsNullOrWhiteSpace(Password);

    private bool CanVerifyPin() =>
        RequiresPinVerification && !string.IsNullOrWhiteSpace(PinSecundario);

    private async Task LoginAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var usuario = await _authenticationService.AutenticarAsync(NombreUsuario, Password);
            if (usuario == null)
            {
                ErrorMessage = "Usuario o contraseña incorrectos";
                return;
            }

            if (UserPreferencesService.TienePinSecundario(usuario.Id))
            {
                _pendingUsuario = usuario;
                PinSecundario = string.Empty;
                RequiresPinVerification = true;
                LoginCommand.NotifyCanExecuteChanged();
                VerifyPinCommand.NotifyCanExecuteChanged();
                return;
            }

            LoginExitoso?.Invoke(this, usuario);
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

    private void VerifyPin()
    {
        if (_pendingUsuario == null) return;

        if (!UserPreferencesService.VerifyPinSecundario(_pendingUsuario.Id, PinSecundario))
        {
            ErrorMessage = "PIN incorrecto";
            return;
        }

        var usuario = _pendingUsuario;
        _pendingUsuario = null;
        PinSecundario = string.Empty;
        RequiresPinVerification = false;
        LoginCommand.NotifyCanExecuteChanged();
        VerifyPinCommand.NotifyCanExecuteChanged();
        ErrorMessage = null;
        LoginExitoso?.Invoke(this, usuario);
    }

    private void VolverAlLogin()
    {
        _pendingUsuario = null;
        PinSecundario = string.Empty;
        ErrorMessage = null;
        RequiresPinVerification = false;
        LoginCommand.NotifyCanExecuteChanged();
        VerifyPinCommand.NotifyCanExecuteChanged();
    }
}
