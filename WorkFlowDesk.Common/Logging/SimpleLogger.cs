using System.IO;

namespace WorkFlowDesk.Common.Logging;

public static class SimpleLogger
{
    private static readonly string LogFilePath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "logs",
        $"log_{DateTime.Now:yyyyMMdd}.txt");

    public static void LogInfo(string message)
    {
        Log("INFO", message);
    }

    public static void LogWarning(string message)
    {
        Log("WARNING", message);
    }

    public static void LogError(string message, Exception? exception = null)
    {
        var errorMessage = message;
        if (exception != null)
        {
            errorMessage += $"\nException: {exception.GetType().Name}\nMessage: {exception.Message}\nStack Trace: {exception.StackTrace}";
        }
        Log("ERROR", errorMessage);
    }

    private static void Log(string level, string message)
    {
        try
        {
            var directory = Path.GetDirectoryName(LogFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}\n";
            File.AppendAllText(LogFilePath, logEntry);
        }
        catch
        {
            // Si no se puede escribir el log, no hacer nada para no interrumpir la aplicación
        }
    }
}
