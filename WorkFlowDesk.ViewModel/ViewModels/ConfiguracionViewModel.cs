using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Configuration;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;
using WorkFlowDesk.ViewModel.Models;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>ViewModel de configuración del sistema.</summary>
public class ConfiguracionViewModel : ViewModelBase, ISearchableViewModel
{
    private readonly IBackupService _backupService;
    private readonly IDatabaseInitializationService _databaseInitializationService;
    private readonly IAuthenticationService _authenticationService;
    private readonly ISyncService _syncService;

    private string _rutaSqlite = string.Empty;
    private string _carpetaCompartida = string.Empty;
    private string _slackWebhookUrl = string.Empty;
    private string _ultimaSyncTexto = "Sin sincronización";
    private string _ultimoBackupTexto = "Sin backups";
    private string _tiempoActividadTexto = string.Empty;
    private string _textoBusqueda = string.Empty;
    private string _passwordActual = string.Empty;
    private string _passwordNuevo = string.Empty;
    private string _passwordConfirmacion = string.Empty;
    private List<string> _backupsDisponibles = new();
    private string? _backupSeleccionado;
    private readonly List<LogActividadItem> _actividadSistema = new();

    public ConfiguracionViewModel(
        IBackupService backupService,
        IDatabaseInitializationService databaseInitializationService,
        IAuthenticationService authenticationService,
        ISyncService syncService)
    {
        _backupService = backupService;
        _databaseInitializationService = databaseInitializationService;
        _authenticationService = authenticationService;
        _syncService = syncService;

        ActividadSistemaFiltrada = new ObservableCollection<LogActividadItem>();

        CargarConfiguracionCommand = new AsyncRelayCommand(CargarConfiguracionAsync);
        CrearBackupCommand = new AsyncRelayCommand(CrearBackupAsync);
        InicializarBaseDatosCommand = new AsyncRelayCommand(InicializarBaseDatosAsync);
        CambiarPasswordCommand = new AsyncRelayCommand(CambiarPasswordAsync, CanCambiarPassword);
        CargarBackupsCommand = new AsyncRelayCommand(CargarBackupsAsync);
        RestaurarBackupCommand = new AsyncRelayCommand(RestaurarBackupAsync, CanRestaurarBackup);
        ExplorarRutaSqliteCommand = new RelayCommand(ExplorarRutaSqlite);
        SolicitarRestaurarBackupCommand = new RelayCommand(() => RestaurarBackupSolicitado?.Invoke(this, EventArgs.Empty));
        ExportarSyncCommand = new AsyncRelayCommand(ExportarSyncAsync);
        ImportarSyncCommand = new AsyncRelayCommand(ImportarSyncAsync);
        GuardarIntegracionesCommand = new RelayCommand(GuardarIntegraciones);
        ExplorarCarpetaSyncCommand = new RelayCommand(() => ExplorarCarpetaSyncSolicitada?.Invoke(this, EventArgs.Empty));

        _ = InicializarVistaAsync();
    }

    public ObservableCollection<LogActividadItem> ActividadSistemaFiltrada { get; }

    public string CarpetaCompartida
    {
        get => _carpetaCompartida;
        set
        {
            SetProperty(ref _carpetaCompartida, value);
            _syncService.CarpetaCompartida = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }

    public string SlackWebhookUrl
    {
        get => _slackWebhookUrl;
        set => SetProperty(ref _slackWebhookUrl, value);
    }

    public string UltimaSyncTexto
    {
        get => _ultimaSyncTexto;
        private set => SetProperty(ref _ultimaSyncTexto, value);
    }

    public string RutaSqlite
    {
        get => _rutaSqlite;
        private set => SetProperty(ref _rutaSqlite, value);
    }

    public string UltimoBackupTexto
    {
        get => _ultimoBackupTexto;
        private set => SetProperty(ref _ultimoBackupTexto, value);
    }

    public string TiempoActividadTexto
    {
        get => _tiempoActividadTexto;
        private set => SetProperty(ref _tiempoActividadTexto, value);
    }

    public string TextoBusqueda
    {
        get => _textoBusqueda;
        set
        {
            if (!SetProperty(ref _textoBusqueda, value))
                return;

            AplicarFiltroActividad();
        }
    }

    public string PasswordActual
    {
        get => _passwordActual;
        set
        {
            SetProperty(ref _passwordActual, value);
            CambiarPasswordCommand.NotifyCanExecuteChanged();
        }
    }

    public string PasswordNuevo
    {
        get => _passwordNuevo;
        set
        {
            SetProperty(ref _passwordNuevo, value);
            CambiarPasswordCommand.NotifyCanExecuteChanged();
        }
    }

    public string PasswordConfirmacion
    {
        get => _passwordConfirmacion;
        set
        {
            SetProperty(ref _passwordConfirmacion, value);
            CambiarPasswordCommand.NotifyCanExecuteChanged();
        }
    }

    public IAsyncRelayCommand CargarConfiguracionCommand { get; }
    public IAsyncRelayCommand CrearBackupCommand { get; }
    public IAsyncRelayCommand InicializarBaseDatosCommand { get; }
    public IAsyncRelayCommand CambiarPasswordCommand { get; }
    public IAsyncRelayCommand CargarBackupsCommand { get; }
    public IAsyncRelayCommand RestaurarBackupCommand { get; }
    public IRelayCommand ExplorarRutaSqliteCommand { get; }
    public IRelayCommand SolicitarRestaurarBackupCommand { get; }
    public IAsyncRelayCommand ExportarSyncCommand { get; }
    public IAsyncRelayCommand ImportarSyncCommand { get; }
    public IRelayCommand GuardarIntegracionesCommand { get; }
    public IRelayCommand ExplorarCarpetaSyncCommand { get; }

    public IEnumerable<string> BackupsDisponibles
    {
        get => _backupsDisponibles;
        private set => SetProperty(ref _backupsDisponibles, value.ToList());
    }

    public string? BackupSeleccionado
    {
        get => _backupSeleccionado;
        set
        {
            SetProperty(ref _backupSeleccionado, value);
            RestaurarBackupCommand.NotifyCanExecuteChanged();
        }
    }

    public event EventHandler<string>? OperacionCompletada;
    public event EventHandler<string>? ExplorarRutaSolicitada;
    public event EventHandler? RestaurarBackupSolicitado;
    public event EventHandler? ExplorarCarpetaSyncSolicitada;

    private static string IntegrationsPath =>
        Path.Combine(DatabasePaths.GetDataDirectory(), "integrations.json");

    private async Task InicializarVistaAsync()
    {
        IsLoading = true;
        try
        {
            RutaSqlite = DatabasePaths.GetDatabaseFilePath();
            CarpetaCompartida = _syncService.CarpetaCompartida ?? string.Empty;
            CargarIntegraciones();
            var ultima = await _syncService.GetUltimaSyncAsync();
            UltimaSyncTexto = ultima.HasValue
                ? $"Última sync: {ultima.Value.ToLocalTime():dd/MM/yyyy HH:mm}"
                : "Sin sincronización";
            TiempoActividadTexto = AppRuntimeInfo.GetUptimeFormatted();
            await CargarBackupsAsync();
            InicializarActividadSistema();
            AplicarFiltroActividad();
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

    private void CargarIntegraciones()
    {
        if (!File.Exists(IntegrationsPath))
            return;

        try
        {
            var json = File.ReadAllText(IntegrationsPath);
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("SlackWebhookUrl", out var url))
                SlackWebhookUrl = url.GetString() ?? string.Empty;
        }
        catch
        {
            // Ignorar JSON inválido.
        }
    }

    private void GuardarIntegraciones()
    {
        try
        {
            var payload = JsonSerializer.Serialize(new { SlackWebhookUrl = SlackWebhookUrl.Trim() },
                new JsonSerializerOptions { WriteIndented = true });
            Directory.CreateDirectory(DatabasePaths.GetDataDirectory());
            File.WriteAllText(IntegrationsPath, payload);
            OperacionCompletada?.Invoke(this, "Integraciones guardadas correctamente.");
            RegistrarActividad("SYNC: Webhook de Slack actualizado en integrations.json");
        }
        catch (Exception ex)
        {
            ExceptionHandler.LogException(ex);
            ErrorMessage = ExceptionHandler.HandleException(ex);
        }
    }

    private async Task ExportarSyncAsync()
    {
        IsLoading = true;
        try
        {
            _syncService.CarpetaCompartida = string.IsNullOrWhiteSpace(CarpetaCompartida) ? null : CarpetaCompartida.Trim();
            var result = await _syncService.ExportarCambiosAsync();
            if (!result.Exito)
            {
                ErrorMessage = result.Mensaje;
                return;
            }

            ErrorMessage = null;
            UltimaSyncTexto = $"Última sync: {DateTime.Now:dd/MM/yyyy HH:mm}";
            OperacionCompletada?.Invoke(this, result.Mensaje);
            RegistrarActividad($"SYNC: Exportación — {result.RegistrosAplicados} registros");
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

    private async Task ImportarSyncAsync()
    {
        IsLoading = true;
        try
        {
            _syncService.CarpetaCompartida = string.IsNullOrWhiteSpace(CarpetaCompartida) ? null : CarpetaCompartida.Trim();
            var result = await _syncService.ImportarCambiosAsync();
            if (!result.Exito)
            {
                ErrorMessage = result.Mensaje;
                return;
            }

            ErrorMessage = null;
            UltimaSyncTexto = $"Última sync: {DateTime.Now:dd/MM/yyyy HH:mm}";
            OperacionCompletada?.Invoke(this, result.Mensaje);
            RegistrarActividad($"SYNC: Importación — {result.RegistrosAplicados} registros aplicados");
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

    private Task CargarConfiguracionAsync() => InicializarVistaAsync();

    private void InicializarActividadSistema()
    {
        _actividadSistema.Clear();
        var ahora = DateTime.Now;

        _actividadSistema.Add(new LogActividadItem
        {
            Hora = ahora.AddHours(-2).ToString("HH:mm:ss"),
            Mensaje = $"BACKUP: Último registro disponible — {UltimoBackupTexto}"
        });
        _actividadSistema.Add(new LogActividadItem
        {
            Hora = ahora.AddHours(-3).ToString("HH:mm:ss"),
            Mensaje = $"SECURITY: Inicio de sesión administrativa — {SessionService.GetUserName()}"
        });
        _actividadSistema.Add(new LogActividadItem
        {
            Hora = ahora.AddHours(-4).ToString("HH:mm:ss"),
            Mensaje = "SYSTEM: Motor SQLite operativo en ruta local de aplicación",
            EsAdvertencia = false
        });

        try
        {
            var drive = Path.GetPathRoot(RutaSqlite);
            if (!string.IsNullOrEmpty(drive))
            {
                var info = new DriveInfo(drive);
                if (info.TotalSize > 0)
                {
                    var usedPct = 100.0 * (info.TotalSize - info.AvailableFreeSpace) / info.TotalSize;
                    if (usedPct >= 80)
                    {
                        _actividadSistema.Add(new LogActividadItem
                        {
                            Hora = ahora.AddHours(-5).ToString("HH:mm:ss"),
                            Mensaje = $"WARNING: Espacio en disco alcanzando el {usedPct:0}% de capacidad",
                            EsAdvertencia = true
                        });
                    }
                }
            }
        }
        catch
        {
            // Ignorar métricas de disco no disponibles.
        }
    }

    private void AplicarFiltroActividad()
    {
        ActividadSistemaFiltrada.Clear();
        var termino = TextoBusqueda.Trim();
        var items = string.IsNullOrEmpty(termino)
            ? _actividadSistema
            : _actividadSistema.Where(l =>
                l.Mensaje.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
                l.Hora.Contains(termino, StringComparison.OrdinalIgnoreCase));

        foreach (var item in items)
            ActividadSistemaFiltrada.Add(item);
    }

    private void ExplorarRutaSqlite()
    {
        var directorio = Path.GetDirectoryName(RutaSqlite);
        if (!string.IsNullOrWhiteSpace(directorio))
            ExplorarRutaSolicitada?.Invoke(this, directorio);
    }

    private void ActualizarUltimoBackup()
    {
        if (_backupsDisponibles.Count == 0)
        {
            UltimoBackupTexto = "Sin backups";
            return;
        }

        var latest = _backupsDisponibles[0];
        UltimoBackupTexto = FormatearTiempoRelativo(File.GetCreationTime(latest));
    }

    private static string FormatearTiempoRelativo(DateTime fecha)
    {
        var diff = DateTime.Now - fecha;
        if (diff.TotalMinutes < 60) return $"hace {(int)Math.Max(1, diff.TotalMinutes)} min";
        if (diff.TotalHours < 24) return $"hace {(int)diff.TotalHours} horas";
        if (diff.TotalDays < 7) return $"hace {(int)diff.TotalDays} días";
        return fecha.ToString("dd MMM yyyy HH:mm");
    }

    private void RegistrarActividad(string mensaje, bool advertencia = false)
    {
        _actividadSistema.Insert(0, new LogActividadItem
        {
            Hora = DateTime.Now.ToString("HH:mm:ss"),
            Mensaje = mensaje,
            EsAdvertencia = advertencia
        });
        TiempoActividadTexto = AppRuntimeInfo.GetUptimeFormatted();
        AplicarFiltroActividad();
    }

    private async Task CrearBackupAsync()
    {
        IsLoading = true;
        try
        {
            var backupPath = await _backupService.CreateBackupAsync();
            ErrorMessage = null;
            OperacionCompletada?.Invoke(this, $"Backup creado correctamente.\n{backupPath}");
            RegistrarActividad($"BACKUP: Generado exitosamente en {backupPath}");
            await CargarBackupsAsync();
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

    private async Task CargarBackupsAsync()
    {
        try
        {
            BackupsDisponibles = await _backupService.GetAvailableBackupsAsync();
            if (BackupSeleccionado == null || !BackupsDisponibles.Contains(BackupSeleccionado))
                BackupSeleccionado = BackupsDisponibles.FirstOrDefault();

            ActualizarUltimoBackup();
        }
        catch (Exception ex)
        {
            ExceptionHandler.LogException(ex);
            ErrorMessage = ExceptionHandler.HandleException(ex);
        }
    }

    private bool CanRestaurarBackup() => !string.IsNullOrWhiteSpace(BackupSeleccionado);

    private async Task RestaurarBackupAsync()
    {
        if (string.IsNullOrWhiteSpace(BackupSeleccionado))
            return;

        if (!SolicitarConfirmacion(
                "¿Restaurar la base de datos desde el backup seleccionado? Se sobrescribirán los datos actuales.",
                "Confirmar restauración"))
            return;

        IsLoading = true;
        try
        {
            var restaurado = await _backupService.RestoreBackupAsync(BackupSeleccionado);
            if (!restaurado)
            {
                ErrorMessage = "No se pudo restaurar el backup seleccionado.";
                return;
            }

            ErrorMessage = null;
            OperacionCompletada?.Invoke(this, "Base de datos restaurada correctamente.");
            RegistrarActividad($"RESTORE: Base de datos restaurada desde {Path.GetFileName(BackupSeleccionado)}");
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

    private async Task InicializarBaseDatosAsync()
    {
        if (!SolicitarConfirmacion(
                "Se eliminarán todos los datos y se recreará la base de datos con datos de prueba. ¿Continuar?",
                "Inicializar base de datos"))
            return;

        IsLoading = true;
        try
        {
            await _databaseInitializationService.InitializeAsync();
            ErrorMessage = null;
            OperacionCompletada?.Invoke(this, "Base de datos inicializada correctamente.");
            RegistrarActividad("DATABASE: Inicialización completa de SQLite ejecutada", advertencia: true);
            RutaSqlite = DatabasePaths.GetDatabaseFilePath();
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

    private bool CanCambiarPassword() =>
        !string.IsNullOrWhiteSpace(PasswordActual) &&
        !string.IsNullOrWhiteSpace(PasswordNuevo) &&
        !string.IsNullOrWhiteSpace(PasswordConfirmacion) &&
        PasswordNuevo.Length >= 6;

    private async Task CambiarPasswordAsync()
    {
        if (PasswordNuevo != PasswordConfirmacion)
        {
            ErrorMessage = "Las contraseñas nuevas no coinciden.";
            return;
        }

        var usuario = SessionService.CurrentUser;
        if (usuario == null)
        {
            ErrorMessage = "No hay sesión activa.";
            return;
        }

        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var cambiado = await _authenticationService.CambiarPasswordAsync(
                usuario.Id, PasswordActual, PasswordNuevo);

            if (!cambiado)
            {
                ErrorMessage = "La contraseña actual es incorrecta.";
                return;
            }

            PasswordActual = string.Empty;
            PasswordNuevo = string.Empty;
            PasswordConfirmacion = string.Empty;
            UserPreferencesService.SetPasswordChangedAt(usuario.Id, DateTime.Now);
            OperacionCompletada?.Invoke(this, "Contraseña actualizada correctamente.");
            RegistrarActividad("SECURITY: Contraseña administrativa actualizada correctamente");
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
