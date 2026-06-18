using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Helpers;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;
using WorkFlowDesk.ViewModel.Models;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>ViewModel del formulario de empleado (alta/edición).</summary>
public class EmpleadoFormViewModel : ViewModelBase
{
    private const string DefaultTemporaryPassword = "Welcome1";
    private readonly IEmpleadoService _empleadoService;
    private readonly IUsuarioService _usuarioService;
    private Empleado _empleado;
    private readonly bool _esNuevo;
    private int _selectedAvatarIndex;
    private RolRegistroOption _rolSeleccionado;

    public EmpleadoFormViewModel(
        IEmpleadoService empleadoService,
        IUsuarioService usuarioService,
        Empleado? empleado = null)
    {
        _empleadoService = empleadoService;
        _usuarioService = usuarioService;
        _empleado = empleado ?? new Empleado
        {
            Estado = EstadoEmpleado.Activo,
            FechaContratacion = DateTime.Now
        };
        _esNuevo = empleado is null || empleado.Id == 0;

        if (_esNuevo && _empleado.AvatarIndex == 0)
            _empleado.AvatarIndex = Random.Shared.Next(0, AvatarCatalog.Count);

        _selectedAvatarIndex = Math.Clamp(_empleado.AvatarIndex, 0, AvatarCatalog.Count - 1);

        Avatares = new ObservableCollection<AvatarOption>(
            Enumerable.Range(0, AvatarCatalog.Count)
                .Select(i => new AvatarOption { Index = i, Url = AvatarCatalog.GetUrl(i) }));

        RolesDisponibles = BuildRolesDisponibles(_empleado);
        _rolSeleccionado = ResolveRolInicial(_empleado);

        GuardarCommand = new AsyncRelayCommand(GuardarAsync, CanGuardar);
        CancelarCommand = new RelayCommand(Cancelar);
        SelectAvatarCommand = new RelayCommand<int>(index => SelectedAvatarIndex = index);
    }

    public string Titulo => _esNuevo ? "Nuevo Empleado" : "Editar Empleado";

    public string Subtitulo => _esNuevo
        ? "Registra un nuevo miembro en el equipo"
        : "Actualiza la información del empleado";

    public ObservableCollection<AvatarOption> Avatares { get; }

    public IReadOnlyList<RolRegistroOption> RolesDisponibles { get; }

    public RolRegistroOption RolSeleccionado
    {
        get => _rolSeleccionado;
        set => SetProperty(ref _rolSeleccionado, value);
    }

    public int SelectedAvatarIndex
    {
        get => _selectedAvatarIndex;
        set
        {
            if (!SetProperty(ref _selectedAvatarIndex, Math.Clamp(value, 0, AvatarCatalog.Count - 1)))
                return;

            _empleado.AvatarIndex = _selectedAvatarIndex;
        }
    }

    public string InicialesVistaPrevia
    {
        get
        {
            var n = string.IsNullOrWhiteSpace(Nombre) ? "?" : Nombre.Trim()[0].ToString();
            var a = string.IsNullOrWhiteSpace(Apellidos) ? string.Empty : Apellidos.Trim()[0].ToString();
            return $"{n}{a}".ToUpperInvariant();
        }
    }

    public bool EstadoActivo
    {
        get => Estado == EstadoEmpleado.Activo;
        set
        {
            if (!value) return;
            Estado = EstadoEmpleado.Activo;
            OnPropertyChanged(nameof(EstadoAusente));
        }
    }

    public bool EstadoAusente
    {
        get => Estado is EstadoEmpleado.Vacaciones or EstadoEmpleado.Inactivo or EstadoEmpleado.Baja;
        set
        {
            if (!value) return;
            Estado = EstadoEmpleado.Vacaciones;
            OnPropertyChanged(nameof(EstadoActivo));
        }
    }

    public string Nombre
    {
        get => _empleado.Nombre;
        set
        {
            _empleado.Nombre = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(InicialesVistaPrevia));
            GuardarCommand.NotifyCanExecuteChanged();
        }
    }

    public string Apellidos
    {
        get => _empleado.Apellidos;
        set
        {
            _empleado.Apellidos = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(InicialesVistaPrevia));
            GuardarCommand.NotifyCanExecuteChanged();
        }
    }

    public string Email
    {
        get => _empleado.Email;
        set
        {
            _empleado.Email = value;
            OnPropertyChanged();
            ValidarEmail();
            GuardarCommand.NotifyCanExecuteChanged();
        }
    }

    private void ValidarEmail()
    {
        if (!string.IsNullOrWhiteSpace(Email) && !ValidationHelper.IsValidEmail(Email))
            ErrorMessage = "El formato del email no es válido";
        else if (string.IsNullOrWhiteSpace(ErrorMessage) || ErrorMessage.Contains("email"))
            ErrorMessage = null;
    }

    public string Telefono
    {
        get => _empleado.Telefono;
        set
        {
            _empleado.Telefono = value;
            OnPropertyChanged();
            ValidarTelefono();
            GuardarCommand.NotifyCanExecuteChanged();
        }
    }

    public string Departamento
    {
        get => _empleado.Departamento;
        set
        {
            _empleado.Departamento = value;
            OnPropertyChanged();
        }
    }

    public string Cargo
    {
        get => _empleado.Cargo;
        set
        {
            _empleado.Cargo = value;
            OnPropertyChanged();
        }
    }

    public EstadoEmpleado Estado
    {
        get => _empleado.Estado;
        set
        {
            _empleado.Estado = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(EstadoActivo));
            OnPropertyChanged(nameof(EstadoAusente));
        }
    }

    public DateTime FechaContratacion
    {
        get => _empleado.FechaContratacion;
        set
        {
            _empleado.FechaContratacion = value;
            OnPropertyChanged();
        }
    }

    public IAsyncRelayCommand GuardarCommand { get; }
    public IRelayCommand CancelarCommand { get; }
    public IRelayCommand<int> SelectAvatarCommand { get; }

    public event EventHandler? Guardado;
    public event EventHandler? Cancelado;

    private bool CanGuardar()
    {
        return RolSeleccionado != null &&
               !string.IsNullOrWhiteSpace(Nombre) &&
               !string.IsNullOrWhiteSpace(Apellidos) &&
               !string.IsNullOrWhiteSpace(Email) &&
               ValidationHelper.IsValidEmail(Email) &&
               (string.IsNullOrWhiteSpace(Telefono) || ValidationHelper.IsValidPhone(Telefono));
    }

    private void ValidarTelefono()
    {
        if (!string.IsNullOrWhiteSpace(Telefono) && !ValidationHelper.IsValidPhone(Telefono))
            ErrorMessage = "El teléfono debe tener al menos 9 dígitos";
        else if (string.IsNullOrWhiteSpace(ErrorMessage) || ErrorMessage.Contains("teléfono"))
            ErrorMessage = null;
    }

    private async Task GuardarAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            if (!SessionService.IsAdmin() && RolSeleccionado.TipoRol == TipoRol.Admin)
                throw new InvalidOperationException("Solo un administrador puede asignar el rol Administrador.");

            _empleado.AvatarIndex = SelectedAvatarIndex;

            if (_esNuevo)
            {
                _empleado = await _empleadoService.CreateAsync(_empleado);
                await CrearUsuarioParaEmpleadoAsync(_empleado);
            }
            else
            {
                await _empleadoService.UpdateAsync(_empleado);
                await SincronizarRolUsuarioAsync(_empleado);
            }

            Guardado?.Invoke(this, EventArgs.Empty);
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

    private async Task CrearUsuarioParaEmpleadoAsync(Empleado empleado)
    {
        if (empleado.UsuarioId.HasValue)
        {
            await _usuarioService.AsignarRolAsync(empleado.UsuarioId.Value, RolSeleccionado.TipoRol);
            return;
        }

        var nombreCompleto = $"{empleado.Nombre} {empleado.Apellidos}".Trim();
        var nombreUsuario = await GenerarNombreUsuarioUnicoAsync(empleado.Email, empleado.Nombre, empleado.Apellidos);
        var usuario = await _usuarioService.RegistrarAsync(
            nombreUsuario,
            empleado.Email,
            nombreCompleto,
            DefaultTemporaryPassword,
            RolSeleccionado.TipoRol);

        empleado.UsuarioId = usuario.Id;
        await _empleadoService.UpdateAsync(empleado);
    }

    private async Task SincronizarRolUsuarioAsync(Empleado empleado)
    {
        if (!empleado.UsuarioId.HasValue)
        {
            await CrearUsuarioParaEmpleadoAsync(empleado);
            return;
        }

        await _usuarioService.AsignarRolAsync(empleado.UsuarioId.Value, RolSeleccionado.TipoRol);
    }

    private async Task<string> GenerarNombreUsuarioUnicoAsync(string email, string nombre, string apellidos)
    {
        var baseName = email.Split('@')[0]
            .Replace(".", "_")
            .Replace("-", "_")
            .ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(baseName))
            baseName = $"{nombre}.{apellidos}".Replace(" ", string.Empty).ToLowerInvariant();

        var candidate = baseName;
        var suffix = 1;
        while (await _usuarioService.ExistsAsync(candidate))
        {
            candidate = $"{baseName}{suffix}";
            suffix++;
        }

        return candidate;
    }

    private static IReadOnlyList<RolRegistroOption> BuildRolesDisponibles(Empleado empleado)
    {
        var roles = new List<RolRegistroOption>
        {
            new(TipoRol.Empleado, "Empleado", "Dashboard y tareas"),
            new(TipoRol.Supervisor, "Supervisor", "Equipo, proyectos y reportes")
        };

        if (SessionService.IsAdmin())
        {
            roles.Add(new RolRegistroOption(TipoRol.Admin, "Administrador", "Acceso completo"));
        }
        else if (empleado.Usuario?.Rol?.TipoRol == TipoRol.Admin)
        {
            roles.Add(new RolRegistroOption(TipoRol.Admin, "Administrador", "Acceso completo"));
        }

        return roles;
    }

    private RolRegistroOption ResolveRolInicial(Empleado empleado)
    {
        var tipoRol = empleado.Usuario?.Rol?.TipoRol ?? TipoRol.Empleado;
        return RolesDisponibles.FirstOrDefault(r => r.TipoRol == tipoRol) ?? RolesDisponibles[0];
    }

    private void Cancelar() => Cancelado?.Invoke(this, EventArgs.Empty);
}
