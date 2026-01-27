using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Data;
using WorkFlowDesk.Services.Interfaces;

namespace WorkFlowDesk.Services.Services;

/// <summary>
/// Servicio de backup y restauración. CreateBackupAsync genera actualmente
/// un archivo JSON con metadatos; una implementación futura puede realizar
/// backup real de la base de datos SQL Server.
/// </summary>
public class BackupService : IBackupService
{
    private readonly ApplicationDbContext _context;

    public BackupService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> CreateBackupAsync(string? backupPath = null)
    {
        var directory = backupPath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var fileName = $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.db";
        var filePath = Path.Combine(directory, fileName);

        // Por ahora solo creamos un archivo de información
        // En una implementación real, se haría un backup de la base de datos SQL Server
        var backupInfo = new
        {
            FechaCreacion = DateTime.Now,
            BaseDatos = _context.Database.GetDbConnection().Database,
            Servidor = _context.Database.GetDbConnection().DataSource
        };

        var infoJson = System.Text.Json.JsonSerializer.Serialize(backupInfo, new System.Text.Json.JsonSerializerOptions 
        { 
            WriteIndented = true 
        });

        await File.WriteAllTextAsync(filePath, infoJson);
        return filePath;
    }

    public Task<bool> RestoreBackupAsync(string backupFilePath)
    {
        // Implementación básica - en producción se restauraría la base de datos
        if (!File.Exists(backupFilePath))
        {
            return Task.FromResult(false);
        }

        // Por ahora solo verificamos que el archivo existe
        return Task.FromResult(true);
    }

    public Task<List<string>> GetAvailableBackupsAsync()
    {
        var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
        if (!Directory.Exists(directory))
        {
            return Task.FromResult(new List<string>());
        }

        var backups = Directory.GetFiles(directory, "backup_*.db")
            .OrderByDescending(f => File.GetCreationTime(f))
            .ToList();

        return Task.FromResult(backups);
    }
}
