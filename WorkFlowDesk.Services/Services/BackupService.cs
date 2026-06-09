using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Data;
using WorkFlowDesk.Services.Interfaces;

namespace WorkFlowDesk.Services.Services;

/// <summary>Servicio de backup y restauración de la base de datos SQL Server.</summary>
public class BackupService : IBackupService
{
    private readonly ApplicationDbContext _context;

    /// <summary>Inicializa el servicio con el contexto de base de datos.</summary>
    public BackupService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>Crea un backup .bak de la base de datos en la carpeta Backups.</summary>
    public async Task<string> CreateBackupAsync(string? backupPath = null)
    {
        var directory = backupPath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
        Directory.CreateDirectory(directory);

        var connection = _context.Database.GetDbConnection();
        await connection.OpenAsync();

        try
        {
            var databaseName = connection.Database;
            var fileName = $"backup_{databaseName}_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
            var filePath = Path.Combine(directory, fileName);

            await using var command = connection.CreateCommand();
            command.CommandText =
                $"BACKUP DATABASE [{databaseName}] TO DISK = @path WITH INIT, COPY_ONLY, FORMAT";
            command.Parameters.Add(new SqlParameter("@path", filePath));
            await command.ExecuteNonQueryAsync();

            return filePath;
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    /// <summary>Restaura la base de datos desde un archivo .bak (requiere permisos elevados en LocalDB).</summary>
    public async Task<bool> RestoreBackupAsync(string backupFilePath)
    {
        if (!File.Exists(backupFilePath))
            return false;

        var connection = _context.Database.GetDbConnection();
        await connection.OpenAsync();

        try
        {
            var databaseName = connection.Database;
            var dataSource = connection.DataSource;

            await using (var setSingleUser = connection.CreateCommand())
            {
                setSingleUser.CommandText =
                    $"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                await setSingleUser.ExecuteNonQueryAsync();
            }

            await using (var restore = connection.CreateCommand())
            {
                restore.CommandText =
                    $"RESTORE DATABASE [{databaseName}] FROM DISK = @path WITH REPLACE";
                restore.Parameters.Add(new SqlParameter("@path", backupFilePath));
                await restore.ExecuteNonQueryAsync();
            }

            await using (var setMultiUser = connection.CreateCommand())
            {
                setMultiUser.CommandText =
                    $"ALTER DATABASE [{databaseName}] SET MULTI_USER";
                await setMultiUser.ExecuteNonQueryAsync();
            }

            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    /// <summary>Lista los backups .bak disponibles en la carpeta Backups.</summary>
    public Task<List<string>> GetAvailableBackupsAsync()
    {
        var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
        if (!Directory.Exists(directory))
            return Task.FromResult(new List<string>());

        var backups = Directory.GetFiles(directory, "backup_*.bak")
            .OrderByDescending(File.GetCreationTime)
            .ToList();

        return Task.FromResult(backups);
    }
}
