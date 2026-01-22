namespace WorkFlowDesk.Services.Interfaces;

public interface IExportService
{
    Task<string> ExportToCsvAsync<T>(IEnumerable<T> data, string fileName);
    Task<string> ExportToTextAsync<T>(IEnumerable<T> data, string fileName, Func<T, string> formatter);
}
