using System.Text;
using WorkFlowDesk.Services.Interfaces;

namespace WorkFlowDesk.Services.Services;

/// <summary>Servicio de exportación a CSV y texto.</summary>
public class ExportService : IExportService
{
    /// <summary>Exporta una colección a un archivo CSV en la carpeta Exports.</summary>
    public async Task<string> ExportToCsvAsync<T>(IEnumerable<T> data, string fileName)
    {
        var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exports");
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var filePath = Path.Combine(directory, $"{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        
        var csv = new StringBuilder();
        var properties = typeof(T).GetProperties();
        
        // Headers
        csv.AppendLine(string.Join(",", properties.Select(p => p.Name)));
        
        // Data
        foreach (var item in data)
        {
            var values = properties.Select(p =>
            {
                var value = p.GetValue(item);
                return value?.ToString()?.Replace(",", ";") ?? string.Empty;
            });
            csv.AppendLine(string.Join(",", values));
        }

        await File.WriteAllTextAsync(filePath, csv.ToString(), Encoding.UTF8);
        return filePath;
    }

    /// <summary>Exporta una colección a un archivo de texto usando el formateador indicado.</summary>
    public async Task<string> ExportToTextAsync<T>(IEnumerable<T> data, string fileName, Func<T, string> formatter)
    {
        var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exports");
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var filePath = Path.Combine(directory, $"{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
        var content = string.Join(Environment.NewLine, data.Select(formatter));
        
        await File.WriteAllTextAsync(filePath, content, Encoding.UTF8);
        return filePath;
    }
}
