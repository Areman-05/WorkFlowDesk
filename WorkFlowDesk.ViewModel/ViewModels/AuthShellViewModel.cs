using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>Coordina la navegación entre login y registro en la misma ventana.</summary>
public sealed class AuthShellViewModel : ViewModelBase
{
    private bool _mostrarRegistro;

    public AuthShellViewModel(IAuthenticationService authService, IUsuarioService usuarioService)
    {
        Login = new LoginViewModel(authService);
        Register = new RegisterViewModel(usuarioService, authService);

        Login.AbrirRegistroRequested += (_, _) => MostrarRegistro = true;
        Register.CancelarRequested += (_, _) => MostrarRegistro = false;
        Login.LoginExitoso += (_, usuario) => AutenticacionExitosa?.Invoke(this, usuario);
        Register.RegistroExitoso += (_, usuario) => AutenticacionExitosa?.Invoke(this, usuario);
    }

    public LoginViewModel Login { get; }
    public RegisterViewModel Register { get; }

    public bool MostrarRegistro
    {
        get => _mostrarRegistro;
        set => SetProperty(ref _mostrarRegistro, value);
    }

    public event EventHandler<Usuario>? AutenticacionExitosa;
}
