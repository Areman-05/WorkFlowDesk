namespace WorkFlowDesk.Services.Interfaces;

/// <summary>Servicio de copias de seguridad y restauración de la base de datos.</summary>
public interface IBackupService
{
    Task<string> CreateBackupAsync(string? backupPath = null);
    Task<bool> RestoreBackupAsync(string backupFilePath);
    Task<List<string>> GetAvailableBackupsAsync();
}
