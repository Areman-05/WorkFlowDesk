using WorkFlowDesk.Services.Services;

namespace WorkFlowDesk.Tests.Services;

public class ExportServiceTests : IDisposable
{
    private readonly string _exportsDirectory;
    private readonly ExportService _service = new();

    public ExportServiceTests()
    {
        _exportsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exports");
    }

    [Fact]
    public async Task ExportToCsvAsync_crea_archivo_con_cabeceras_y_datos()
    {
        var datos = new[]
        {
            new ExportTestRow { Nombre = "Alpha", Valor = 10 },
            new ExportTestRow { Nombre = "Beta, Inc", Valor = 20 }
        };

        var path = await _service.ExportToCsvAsync(datos, "test_export");

        Assert.True(File.Exists(path));
        var contenido = await File.ReadAllTextAsync(path);
        Assert.Contains("Nombre,Valor", contenido);
        Assert.Contains("Alpha,10", contenido);
        Assert.Contains("Beta; Inc,20", contenido);
    }

    [Fact]
    public async Task ExportToTextAsync_crea_archivo_con_formato_personalizado()
    {
        var datos = new[] { "uno", "dos" };

        var path = await _service.ExportToTextAsync(datos, "test_text", s => $"[{s}]");

        Assert.True(File.Exists(path));
        var contenido = await File.ReadAllTextAsync(path);
        Assert.Equal($"[uno]{Environment.NewLine}[dos]", contenido);
    }

    public void Dispose()
    {
        if (!Directory.Exists(_exportsDirectory))
        {
            return;
        }

        foreach (var file in Directory.GetFiles(_exportsDirectory, "test_*"))
        {
            File.Delete(file);
        }
    }

    private sealed class ExportTestRow
    {
        public string Nombre { get; set; } = string.Empty;
        public int Valor { get; set; }
    }
}
