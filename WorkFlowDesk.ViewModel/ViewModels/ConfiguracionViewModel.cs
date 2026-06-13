using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Configuration;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>ViewModel de configuración del sistema.</summary>
public class ConfiguracionViewModel : ViewModelBase
{
    private readonly IBackupService _backupService;
    private readonly IDatabaseInitializationService _databaseInitializationService;
    private readonly IAuthenticationService _authenticationService;
    private int _defaultPageSize;
    private bool _enableLogging;
    private string _logLevel = string.Empty;
    private int _cacheExpirationMinutes;
    private string _passwordActual = string.Empty;
    private string _passwordNuevo = string.Empty;
    private string _passwordConfirmacion = string.Empty;
    private List<string> _backupsDisponibles = new();
    private string? _backupSeleccionado;

    /// <summary>Construye el ViewModel e inicia la carga de configuración.</summary>
    public ConfiguracionViewModel(
        IBackupService backupService,
        IDatabaseInitializationService databaseInitializationService,
        IAuthenticationService authenticationService)
    {
        _backupService = backupService;
        _databaseInitializationService = databaseInitializationService;
        _authenticationService = authenticationService;

        CargarConfiguracionCommand = new CommunityToolkit.Mvvm.Input.AsyncRelayCommand(CargarConfiguracionAsync);
        GuardarConfiguracionCommand = new RelayCommand(GuardarConfiguracion);
        CrearBackupCommand = new CommunityToolkit.Mvvm.Input.AsyncRelayCommand(CrearBackupAsync);
        InicializarBaseDatosCommand = new CommunityToolkit.Mvvm.Input.AsyncRelayCommand(InicializarBaseDatosAsync);
        CambiarPasswordCommand = new CommunityToolkit.Mvvm.Input.AsyncRelayCommand(CambiarPasswordAsync, CanCambiarPassword);
        CargarBackupsCommand = new CommunityToolkit.Mvvm.Input.AsyncRelayCommand(CargarBackupsAsync);
        RestaurarBackupCommand = new CommunityToolkit.Mvvm.Input.AsyncRelayCommand(RestaurarBackupAsync, CanRestaurarBackup);

        CargarConfiguracionCommand.ExecuteAsync(null);
        CargarBackupsCommand.ExecuteAsync(null);
    }

    public int DefaultPageSize
    {
        get => _defaultPageSize;
        set => SetProperty(ref _defaultPageSize, value);
    }

    public bool EnableLogging
    {
        get => _enableLogging;
        set => SetProperty(ref _enableLogging, value);
    }

    public IReadOnlyList<string> NivelesLog { get; } = new[] { "Debug", "Info", "Warning", "Error" };

    public string LogLevel
    {
        get => _logLevel;
        set => SetProperty(ref _logLevel, value);
    }

    public int CacheExpirationMinutes
    {
        get => _cacheExpirationMinutes;
        set => SetProperty(ref _cacheExpirationMinutes, value);
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

    public CommunityToolkit.Mvvm.Input.IAsyncRelayCommand CargarConfiguracionCommand { get; }
    public IRelayCommand GuardarConfiguracionCommand { get; }
    public CommunityToolkit.Mvvm.Input.IAsyncRelayCommand CrearBackupCommand { get; }
    public CommunityToolkit.Mvvm.Input.IAsyncRelayCommand InicializarBaseDatosCommand { get; }
    public CommunityToolkit.Mvvm.Input.IAsyncRelayCommand CambiarPasswordCommand { get; }
    public CommunityToolkit.Mvvm.Input.IAsyncRelayCommand CargarBackupsCommand { get; }
    public CommunityToolkit.Mvvm.Input.IAsyncRelayCommand RestaurarBackupCommand { get; }

    public IEnumerable<string> BackupsDisponibles
    {
        get => _backupsDisponibles;
        set => SetProperty(ref _backupsDisponibles, value.ToList());
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

    private Task CargarConfiguracionAsync()
    {
        IsLoading = true;
        try
        {
            var settings = AppConfig.Settings;
            DefaultPageSize = settings.DefaultPageSize;
            EnableLogging = settings.EnableLogging;
            LogLevel = settings.LogLevel;
            CacheExpirationMinutes = settings.CacheExpirationMinutes;
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

        return Task.CompletedTask;
    }

    private void GuardarConfiguracion()
    {
        try
        {
            var settings = AppConfig.Settings;
            settings.DefaultPageSize = DefaultPageSize;
            settings.EnableLogging = EnableLogging;
            settings.LogLevel = LogLevel;
            settings.CacheExpirationMinutes = CacheExpirationMinutes;
            AppConfig.SaveToFile();
            ErrorMessage = null;
            OperacionCompletada?.Invoke(this, "Configuración guardada correctamente.");
        }
        catch (Exception ex)
        {
            ExceptionHandler.LogException(ex);
            ErrorMessage = ExceptionHandler.HandleException(ex);
        }
    }

    private async Task CrearBackupAsync()
    {
        IsLoading = true;
        try
        {
            var backupPath = await _backupService.CreateBackupAsync();
            ErrorMessage = null;
            OperacionCompletada?.Invoke(this, $"Backup creado correctamente.\n{backupPath}");
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
            {
                BackupSeleccionado = BackupsDisponibles.FirstOrDefault();
            }
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
        {
            return;
        }

        IsLoading = true;
        try
        {
            await _databaseInitializationService.InitializeAsync();
            ErrorMessage = null;
            OperacionCompletada?.Invoke(this, "Base de datos inicializada correctamente.");
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
            OperacionCompletada?.Invoke(this, "Contraseña actualizada correctamente.");
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
