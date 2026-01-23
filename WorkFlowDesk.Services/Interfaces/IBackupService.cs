namespace WorkFlowDesk.Services.Interfaces;

public interface IBackupService
{
    Task<string> CreateBackupAsync(string? backupPath = null);
    Task<bool> RestoreBackupAsync(string backupFilePath);
    Task<List<string>> GetAvailableBackupsAsync();
}
