using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Models;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;
using WorkFlowDesk.ViewModel.Models;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>ViewModel del perfil de usuario.</summary>
public class ProfileViewModel : ViewModelBase, ISearchableViewModel
{
    private readonly IEmpleadoService _empleadoService;
    private readonly IUsuarioService _usuarioService;
    private readonly IExportService _exportService;

    private int _selectedAvatarIndex;
    private string _nombreCompleto = string.Empty;
    private string _email = string.Empty;
    private string _telefono = string.Empty;
    private string _ubicacion = string.Empty;
    private string _idioma = "Español (España)";
    private string _tema = "Claro";
    private bool _notificacionesEscritorio = true;
    private bool _autenticacionDosFactores;
    private bool _mostrarConfiguracionPin;
    private string _nuevoPin = string.Empty;
    private string _confirmarPin = string.Empty;
    private string _ultimaActualizacionPassword = "Sin registros";
    private string _cargoDisplay = string.Empty;
    private string _textoBusqueda = string.Empty;
    private bool _mostrarSelectorAvatar;
    private string? _successMessage;
    private Empleado? _empleado;
    private Usuario? _usuario;
    private UserProfileData? _profileData;

    public ProfileViewModel(
        IEmpleadoService empleadoService,
        IUsuarioService usuarioService,
        IExportService exportService)
    {
        _empleadoService = empleadoService;
        _usuarioService = usuarioService;
        _exportService = exportService;

        UserRole = SessionService.GetUserRole();
        Avatares = new ObservableCollection<AvatarOption>(
            Enumerable.Range(0, AvatarCatalog.Count)
                .Select(i => new AvatarOption { Index = i, Url = AvatarCatalog.GetUrl(i) }));
        SesionesActivas = new ObservableCollection<SesionActivaItem>();
        SesionesFiltradas = new ObservableCollection<SesionActivaItem>();
        IdiomasDisponibles = new[] { "Español (España)", "English (US)", "Deutsch" };

        GuardarCambiosCommand = new AsyncRelayCommand(GuardarCambiosAsync);
        DescargarReporteCommand = new AsyncRelayCommand(DescargarReporteAsync);
        EditarAvatarCommand = new RelayCommand(() => MostrarSelectorAvatar = !MostrarSelectorAvatar);
        CambiarPasswordCommand = new RelayCommand(() => AppNavigationService.RequestSection("Configuracion"));
        FinalizarSesionCommand = new RelayCommand<SesionActivaItem>(FinalizarSesion);
        CerrarOtrasSesionesCommand = new RelayCommand(CerrarOtrasSesiones);
        DesactivarCuentaCommand = new AsyncRelayCommand(DesactivarCuentaAsync);
        SeleccionarTemaCommand = new RelayCommand<string>(SeleccionarTema);
        ConfigurarPinCommand = new RelayCommand(ConfigurarPin);
        SelectAvatarCommand = new RelayCommand<int>(index => SelectedAvatarIndex = index);

        _ = CargarPerfilAsync();
    }

    public string UserRole { get; }
    public IReadOnlyList<string> IdiomasDisponibles { get; }
    public ObservableCollection<AvatarOption> Avatares { get; }
    public ObservableCollection<SesionActivaItem> SesionesActivas { get; }
    public ObservableCollection<SesionActivaItem> SesionesFiltradas { get; }

    public string NombreCompleto
    {
        get => _nombreCompleto;
        set => SetProperty(ref _nombreCompleto, value);
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Telefono
    {
        get => _telefono;
        set => SetProperty(ref _telefono, value);
    }

    public string Ubicacion
    {
        get => _ubicacion;
        set => SetProperty(ref _ubicacion, value);
    }

    public string Idioma
    {
        get => _idioma;
        set
        {
            if (!SetProperty(ref _idioma, value))
                return;

            LocalizationService.Apply(value);
            IdiomaChanged?.Invoke(this, value);
        }
    }

    public string Tema
    {
        get => _tema;
        set
        {
            if (!SetProperty(ref _tema, value))
                return;

            OnPropertyChanged(nameof(TemaClaroSeleccionado));
            OnPropertyChanged(nameof(TemaOscuroSeleccionado));
            TemaChanged?.Invoke(this, value);
        }
    }

    public bool NotificacionesEscritorio
    {
        get => _notificacionesEscritorio;
        set
        {
            if (!SetProperty(ref _notificacionesEscritorio, value))
                return;

            NotificacionesChanged?.Invoke(this, value);
        }
    }

    public bool AutenticacionDosFactores
    {
        get => _autenticacionDosFactores;
        set
        {
            if (!SetProperty(ref _autenticacionDosFactores, value))
                return;

            OnPropertyChanged(nameof(EstadoDosFactoresTexto));
            OnPropertyChanged(nameof(TienePinConfigurado));
            OnPropertyChanged(nameof(MostrarConfiguracionPin));

            if (value && _usuario != null && !UserPreferencesService.TienePinSecundario(_usuario.Id))
                MostrarConfiguracionPin = true;
            else if (!value)
                MostrarConfiguracionPin = false;

            PersistirPreferenciasParciales();
        }
    }

    public bool MostrarConfiguracionPin
    {
        get => _mostrarConfiguracionPin;
        set => SetProperty(ref _mostrarConfiguracionPin, value);
    }

    public string NuevoPin
    {
        get => _nuevoPin;
        set => SetProperty(ref _nuevoPin, value);
    }

    public string ConfirmarPin
    {
        get => _confirmarPin;
        set => SetProperty(ref _confirmarPin, value);
    }

    public bool TienePinConfigurado =>
        _usuario != null && UserPreferencesService.TienePinSecundario(_usuario.Id);

    public string UltimaActualizacionPassword
    {
        get => _ultimaActualizacionPassword;
        private set => SetProperty(ref _ultimaActualizacionPassword, value);
    }

    public string EstadoDosFactoresTexto => AutenticacionDosFactores ? "ACTIVO" : "INACTIVO";

    public string CargoDisplay
    {
        get => _cargoDisplay;
        private set => SetProperty(ref _cargoDisplay, value);
    }

    public string TextoBusqueda
    {
        get => _textoBusqueda;
        set
        {
            if (!SetProperty(ref _textoBusqueda, value))
                return;

            AplicarFiltroSesiones();
        }
    }

    public bool MostrarSelectorAvatar
    {
        get => _mostrarSelectorAvatar;
        set => SetProperty(ref _mostrarSelectorAvatar, value);
    }

    public int SelectedAvatarIndex
    {
        get => _selectedAvatarIndex;
        set => SetProperty(ref _selectedAvatarIndex, value);
    }

    public string? SuccessMessage
    {
        get => _successMessage;
        set => SetProperty(ref _successMessage, value);
    }

    public bool TemaClaroSeleccionado => Tema == "Claro";
    public bool TemaOscuroSeleccionado => Tema == "Oscuro";
    public bool MostrarVerificado => _usuario?.Activo == true;

    public IAsyncRelayCommand GuardarCambiosCommand { get; }
    public IAsyncRelayCommand DescargarReporteCommand { get; }
    public IRelayCommand EditarAvatarCommand { get; }
    public IRelayCommand CambiarPasswordCommand { get; }
    public IRelayCommand<SesionActivaItem> FinalizarSesionCommand { get; }
    public IRelayCommand CerrarOtrasSesionesCommand { get; }
    public IAsyncRelayCommand DesactivarCuentaCommand { get; }
    public IRelayCommand<string> SeleccionarTemaCommand { get; }
    public IRelayCommand ConfigurarPinCommand { get; }
    public IRelayCommand<int> SelectAvatarCommand { get; }

    public event EventHandler<string>? OperacionCompletada;
    public event EventHandler<string>? ExportacionCompletada;
    public event EventHandler<string>? TemaChanged;
    public event EventHandler<string>? IdiomaChanged;
    public event EventHandler<bool>? NotificacionesChanged;
    public event EventHandler? CuentaDesactivada;

    private void SeleccionarTema(string? tema)
    {
        if (string.IsNullOrEmpty(tema) || tema == Tema)
            return;

        Tema = tema;
        PersistirPreferenciasParciales();
    }

    private async Task CargarPerfilAsync()
    {
        IsLoading = true;
        try
        {
            var user = SessionService.CurrentUser;
            if (user == null) return;

            _usuario = await _usuarioService.GetByIdAsync(user.Id);
            if (_usuario == null) return;

            NombreCompleto = _usuario.NombreCompleto;
            Email = _usuario.Email;
            SelectedAvatarIndex = UserPreferencesService.GetAvatarIndex(user.Id);

            _profileData = UserPreferencesService.GetProfileData(user.Id);
            Telefono = _profileData.Telefono;
            Ubicacion = _profileData.Ubicacion;
            _idioma = _profileData.Idioma;
            OnPropertyChanged(nameof(Idioma));
            _tema = _profileData.Tema is "Oscuro" ? "Oscuro" : "Claro";
            OnPropertyChanged(nameof(Tema));
            OnPropertyChanged(nameof(TemaClaroSeleccionado));
            OnPropertyChanged(nameof(TemaOscuroSeleccionado));
            _notificacionesEscritorio = _profileData.NotificacionesEscritorio;
            OnPropertyChanged(nameof(NotificacionesEscritorio));
            NotificacionesChanged?.Invoke(this, NotificacionesEscritorio);
            AutenticacionDosFactores = _profileData.AutenticacionDosFactores;
            OnPropertyChanged(nameof(TienePinConfigurado));
            ActualizarTextoPassword(_profileData.PasswordChangedAt);
            LocalizationService.Apply(Idioma);

            var empleados = await _empleadoService.GetAllAsync();
            _empleado = empleados.FirstOrDefault(e => e.UsuarioId == user.Id);
            if (_empleado != null)
            {
                if (string.IsNullOrWhiteSpace(Telefono))
                    Telefono = _empleado.Telefono;
                CargoDisplay = string.IsNullOrWhiteSpace(_empleado.Cargo) ? UserRole : _empleado.Cargo;
            }
            else
            {
                CargoDisplay = UserRole;
            }

            CargarSesiones();
            AplicarFiltroSesiones();
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

    private void ActualizarTextoPassword(DateTime? changedAt)
    {
        if (!changedAt.HasValue)
        {
            UltimaActualizacionPassword = "Sin registros de cambio";
            return;
        }

        var dias = (DateTime.Now - changedAt.Value).Days;
        UltimaActualizacionPassword = dias switch
        {
            0 => "Actualizada hoy",
            1 => "Actualizada ayer",
            < 30 => $"Actualizada hace {dias} días",
            < 60 => "Actualizada hace 1 mes",
            _ => $"Actualizada hace {dias / 30} meses"
        };
    }

    private void CargarSesiones()
    {
        SesionesActivas.Clear();
        var user = SessionService.CurrentUser;
        if (user == null || _profileData == null) return;

        var dispositivoActual = $"{Environment.UserName} en {Environment.OSVersion.VersionString.Split(' ')[0]}";
        SesionesActivas.Add(new SesionActivaItem
        {
            Id = "actual",
            Dispositivo = dispositivoActual,
            Detalle = $"{Ubicacion} • Sesión actual",
            Icono = "\uE770",
            EsActual = true,
            EsDemo = false
        });
    }

    private static string FormatearDetalleSesion(SesionPerfilData sesion)
    {
        if (!string.IsNullOrWhiteSpace(sesion.Detalle))
            return sesion.Detalle;

        var diff = DateTime.Now - sesion.UltimaActividad;
        return diff.TotalHours < 24
            ? $"Activa hace {(int)diff.TotalHours} horas"
            : $"Activa hace {(int)diff.TotalDays} días";
    }

    private void AplicarFiltroSesiones()
    {
        SesionesFiltradas.Clear();
        var termino = TextoBusqueda.Trim();
        var items = string.IsNullOrEmpty(termino)
            ? SesionesActivas
            : SesionesActivas.Where(s =>
                s.Dispositivo.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
                s.Detalle.Contains(termino, StringComparison.OrdinalIgnoreCase));

        foreach (var item in items)
            SesionesFiltradas.Add(item);
    }

    private async Task GuardarCambiosAsync()
    {
        if (_usuario == null) return;

        IsLoading = true;
        ErrorMessage = null;
        try
        {
            _usuario.NombreCompleto = NombreCompleto.Trim();
            _usuario.Email = Email.Trim();
            await _usuarioService.UpdateAsync(_usuario);

            if (_empleado != null)
            {
                _empleado.Telefono = Telefono.Trim();
                _empleado.Email = Email.Trim();
                await _empleadoService.UpdateAsync(_empleado);
            }

            UserPreferencesService.SetAvatarIndex(_usuario.Id, SelectedAvatarIndex);
            PersistirPreferenciasParciales();

            SuccessMessage = "Perfil actualizado correctamente.";
            MostrarSelectorAvatar = false;
            OperacionCompletada?.Invoke(this, SuccessMessage);
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

    private void PersistirPreferenciasParciales()
    {
        if (_usuario == null) return;

        _profileData = new UserProfileData
        {
            Telefono = Telefono.Trim(),
            Ubicacion = Ubicacion.Trim(),
            Idioma = Idioma,
            Tema = Tema,
            NotificacionesEscritorio = NotificacionesEscritorio,
            AutenticacionDosFactores = AutenticacionDosFactores,
            PinSecundario = _profileData?.PinSecundario,
            PasswordChangedAt = _profileData?.PasswordChangedAt,
            Sesiones = _profileData?.Sesiones ?? new List<SesionPerfilData>(),
            SesionesRevocadas = _profileData?.SesionesRevocadas ?? new List<string>()
        };

        UserPreferencesService.SetProfileData(_usuario.Id, _profileData);
    }

    private async Task DescargarReporteAsync()
    {
        IsLoading = true;
        try
        {
            var filas = new List<ProfileExportRow>
            {
                new() { Campo = "Nombre completo", Valor = NombreCompleto },
                new() { Campo = "Email", Valor = Email },
                new() { Campo = "Teléfono", Valor = Telefono },
                new() { Campo = "Ubicación", Valor = Ubicacion },
                new() { Campo = "Cargo", Valor = CargoDisplay },
                new() { Campo = "Rol", Valor = UserRole },
                new() { Campo = "Idioma", Valor = Idioma },
                new() { Campo = "Tema", Valor = Tema },
                new() { Campo = "Notificaciones escritorio", Valor = NotificacionesEscritorio ? "Sí" : "No" },
                new() { Campo = "Autenticación 2FA", Valor = AutenticacionDosFactores ? "Activa" : "Inactiva" },
                new() { Campo = "Contraseña", Valor = UltimaActualizacionPassword },
                new() { Campo = "Sesiones activas", Valor = SesionesActivas.Count.ToString() }
            };

            var path = await _exportService.ExportToCsvAsync(filas, "perfil");
            ExportacionCompletada?.Invoke(this, path);
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

    private void FinalizarSesion(SesionActivaItem? sesion)
    {
        if (sesion == null || sesion.EsActual || _profileData == null || _usuario == null) return;

        if (!SolicitarConfirmacion(
                $"¿Finalizar la sesión en «{sesion.Dispositivo}»?",
                "Finalizar sesión"))
            return;

        if (!_profileData.SesionesRevocadas.Contains(sesion.Id))
            _profileData.SesionesRevocadas.Add(sesion.Id);

        SesionesActivas.Remove(sesion);
        AplicarFiltroSesiones();
        PersistirPreferenciasParciales();
        OperacionCompletada?.Invoke(this, $"Sesión «{sesion.Dispositivo}» finalizada.");
    }

    private void CerrarOtrasSesiones()
    {
        if (_profileData == null || _usuario == null) return;

        if (!SolicitarConfirmacion(
                "¿Cerrar todas las demás sesiones activas?",
                "Cerrar sesiones"))
            return;

        var otras = SesionesActivas.Where(s => !s.EsActual).ToList();
        foreach (var sesion in otras)
        {
            if (!_profileData.SesionesRevocadas.Contains(sesion.Id))
                _profileData.SesionesRevocadas.Add(sesion.Id);
            SesionesActivas.Remove(sesion);
        }

        AplicarFiltroSesiones();
        PersistirPreferenciasParciales();
        OperacionCompletada?.Invoke(this, "Se han cerrado las demás sesiones activas.");
    }

    private void ConfigurarPin()
    {
        if (_usuario == null) return;

        if (string.IsNullOrWhiteSpace(NuevoPin) && TienePinConfigurado)
        {
            MostrarConfiguracionPin = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(NuevoPin) || NuevoPin.Length < 4)
        {
            ErrorMessage = "El PIN debe tener al menos 4 dígitos.";
            return;
        }

        if (NuevoPin != ConfirmarPin)
        {
            ErrorMessage = "Los PIN no coinciden.";
            return;
        }

        UserPreferencesService.SetPinSecundario(_usuario.Id, NuevoPin);
        NuevoPin = string.Empty;
        ConfirmarPin = string.Empty;
        MostrarConfiguracionPin = false;
        OnPropertyChanged(nameof(TienePinConfigurado));
        OperacionCompletada?.Invoke(this, "PIN de verificación configurado correctamente.");
    }

    private async Task DesactivarCuentaAsync()
    {
        if (_usuario == null) return;

        if (!SolicitarConfirmacion(
                "¿Desea desactivar su cuenta? Se cerrará la sesión y un administrador deberá reactivarla.",
                "Desactivar cuenta"))
            return;

        IsLoading = true;
        try
        {
            await _usuarioService.DeleteAsync(_usuario.Id);
            CuentaDesactivada?.Invoke(this, EventArgs.Empty);
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
