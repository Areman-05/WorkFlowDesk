using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Configuration;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.ViewModel.ViewModels;

public class ConfiguracionViewModel : ViewModelBase
{
    private readonly IBackupService _backupService;
    private readonly IDatabaseInitializationService _databaseInitializationService;
    private int _defaultPageSize;
    private bool _enableLogging;
    private string _logLevel = string.Empty;
    private int _cacheExpirationMinutes;

    public ConfiguracionViewModel(
        IBackupService backupService,
        IDatabaseInitializationService databaseInitializationService)
    {
        _backupService = backupService;
        _databaseInitializationService = databaseInitializationService;

        CargarConfiguracionCommand = new CommunityToolkit.Mvvm.Input.AsyncRelayCommand(CargarConfiguracionAsync);
        GuardarConfiguracionCommand = new RelayCommand(GuardarConfiguracion);
        CrearBackupCommand = new CommunityToolkit.Mvvm.Input.AsyncRelayCommand(CrearBackupAsync);
        InicializarBaseDatosCommand = new CommunityToolkit.Mvvm.Input.AsyncRelayCommand(InicializarBaseDatosAsync);

        CargarConfiguracionCommand.ExecuteAsync(null);
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

    public CommunityToolkit.Mvvm.Input.IAsyncRelayCommand CargarConfiguracionCommand { get; }
    public IRelayCommand GuardarConfiguracionCommand { get; }
    public CommunityToolkit.Mvvm.Input.IAsyncRelayCommand CrearBackupCommand { get; }
    public CommunityToolkit.Mvvm.Input.IAsyncRelayCommand InicializarBaseDatosCommand { get; }

    private async Task CargarConfiguracionAsync()
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
            ErrorMessage = $"Error al cargar configuración: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
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
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al guardar configuración: {ex.Message}";
        }
    }

    private async Task CrearBackupAsync()
    {
        IsLoading = true;
        try
        {
            var backupPath = await _backupService.CreateBackupAsync();
            ErrorMessage = null;
            // Aquí se podría mostrar un mensaje de éxito
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al crear backup: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task InicializarBaseDatosAsync()
    {
        IsLoading = true;
        try
        {
            await _databaseInitializationService.InitializeAsync();
            ErrorMessage = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al inicializar base de datos: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
