using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Common.Configuration;
using WorkFlowDesk.Data;
using WorkFlowDesk.Services.Interfaces;

namespace WorkFlowDesk.Services.Services;

/// <summary>Backup y restauración copiando el archivo SQLite.</summary>
public class BackupService : IBackupService
{
    private readonly ApplicationDbContext _context;

    public BackupService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> CreateBackupAsync(string? backupPath = null)
    {
        var sourcePath = DatabasePaths.GetDatabaseFilePath();
        if (!File.Exists(sourcePath))
            throw new FileNotFoundException("No se encontró la base de datos para respaldar.", sourcePath);

        var directory = backupPath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
        Directory.CreateDirectory(directory);

        await _context.Database.CloseConnectionAsync();

        var fileName = $"backup_{Path.GetFileNameWithoutExtension(sourcePath)}_{DateTime.Now:yyyyMMdd_HHmmss}.db";
        var destinationPath = Path.Combine(directory, fileName);
        File.Copy(sourcePath, destinationPath, overwrite: true);

        return destinationPath;
    }

    public async Task<bool> RestoreBackupAsync(string backupFilePath)
    {
        if (!File.Exists(backupFilePath))
            return false;

        var destinationPath = DatabasePaths.GetDatabaseFilePath();
        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);

        try
        {
            await _context.Database.CloseConnectionAsync();
            File.Copy(backupFilePath, destinationPath, overwrite: true);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public Task<List<string>> GetAvailableBackupsAsync()
    {
        var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
        if (!Directory.Exists(directory))
            return Task.FromResult(new List<string>());

        var backups = Directory.GetFiles(directory, "backup_*.db")
            .Concat(Directory.GetFiles(directory, "backup_*.bak"))
            .Distinct()
            .OrderByDescending(File.GetCreationTime)
            .ToList();

        return Task.FromResult(backups);
    }
}
