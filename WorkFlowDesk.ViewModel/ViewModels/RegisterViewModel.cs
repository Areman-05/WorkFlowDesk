using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Helpers;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>Opción de rol mostrada en el formulario de registro.</summary>
public sealed class RolRegistroOption
{
    public RolRegistroOption(TipoRol tipoRol, string nombre, string descripcion)
    {
        TipoRol = tipoRol;
        Nombre = nombre;
        Descripcion = descripcion;
    }

    public TipoRol TipoRol { get; }
    public string Nombre { get; }
    public string Descripcion { get; }
    public string Etiqueta => $"{Nombre} — {Descripcion}";
}

/// <summary>ViewModel del formulario de registro de usuario.</summary>
public class RegisterViewModel : ViewModelBase
{
    private readonly IUsuarioService _usuarioService;
    private readonly IAuthenticationService _authenticationService;
    private string _nombreCompleto = string.Empty;
    private string _nombreUsuario = string.Empty;
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _confirmPassword = string.Empty;
    private bool _aceptaTerminos;
    private RolRegistroOption _rolSeleccionado;

    public RegisterViewModel(IUsuarioService usuarioService, IAuthenticationService authenticationService)
    {
        _usuarioService = usuarioService;
        _authenticationService = authenticationService;
        RolesDisponibles = new[]
        {
            new RolRegistroOption(TipoRol.Empleado, "Empleado", "Dashboard y tareas"),
            new RolRegistroOption(TipoRol.Supervisor, "Supervisor", "Equipo, proyectos y reportes"),
            new RolRegistroOption(TipoRol.Admin, "Administrador", "Acceso completo")
        };
        _rolSeleccionado = RolesDisponibles[0];

        RegisterCommand = new AsyncRelayCommand(RegisterAsync, CanRegister);
        CancelarCommand = new RelayCommand(() => CancelarRequested?.Invoke(this, EventArgs.Empty));
    }

    public IReadOnlyList<RolRegistroOption> RolesDisponibles { get; }

    public RolRegistroOption RolSeleccionado
    {
        get => _rolSeleccionado;
        set
        {
            if (SetProperty(ref _rolSeleccionado, value))
                RegisterCommand.NotifyCanExecuteChanged();
        }
    }

    public string NombreCompleto
    {
        get => _nombreCompleto;
        set
        {
            SetProperty(ref _nombreCompleto, value);
            RegisterCommand.NotifyCanExecuteChanged();
        }
    }

    public string NombreUsuario
    {
        get => _nombreUsuario;
        set
        {
            SetProperty(ref _nombreUsuario, value);
            RegisterCommand.NotifyCanExecuteChanged();
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            SetProperty(ref _email, value);
            RegisterCommand.NotifyCanExecuteChanged();
        }
    }

    public string Password
    {
        get => _password;
        private set
        {
            SetProperty(ref _password, value);
            RegisterCommand.NotifyCanExecuteChanged();
        }
    }

    public string ConfirmPassword
    {
        get => _confirmPassword;
        private set
        {
            SetProperty(ref _confirmPassword, value);
            RegisterCommand.NotifyCanExecuteChanged();
        }
    }

    public bool AceptaTerminos
    {
        get => _aceptaTerminos;
        set
        {
            SetProperty(ref _aceptaTerminos, value);
            RegisterCommand.NotifyCanExecuteChanged();
        }
    }

    public IAsyncRelayCommand RegisterCommand { get; }
    public IRelayCommand CancelarCommand { get; }

    public event EventHandler<Usuario>? RegistroExitoso;
    public event EventHandler? CancelarRequested;

    /// <summary>Asigna contraseñas desde la vista (PasswordBox).</summary>
    public void SetPasswordsFromView(string password, string confirmPassword)
    {
        Password = password ?? string.Empty;
        ConfirmPassword = confirmPassword ?? string.Empty;
    }

    private bool CanRegister() =>
        AceptaTerminos &&
        !string.IsNullOrWhiteSpace(NombreCompleto) &&
        !string.IsNullOrWhiteSpace(NombreUsuario) &&
        !string.IsNullOrWhiteSpace(Email) &&
        !string.IsNullOrWhiteSpace(Password) &&
        !string.IsNullOrWhiteSpace(ConfirmPassword);

    private async Task RegisterAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Las contraseñas no coinciden";
                return;
            }

            await _usuarioService.RegistrarAsync(
                NombreUsuario,
                Email,
                NombreCompleto,
                Password,
                RolSeleccionado.TipoRol);

            var usuario = await _authenticationService.AutenticarAsync(NombreUsuario, Password);
            if (usuario == null)
            {
                ErrorMessage = "Cuenta creada, pero no se pudo iniciar sesión automáticamente";
                return;
            }

            RegistroExitoso?.Invoke(this, usuario);
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
