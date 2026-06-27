using WorkFlowDesk.Common.Models;
using WorkFlowDesk.Common.Services;

namespace WorkFlowDesk.Tests.Services;

public class FlujoWorkflowServiceTests : IDisposable
{
    private readonly string _tempDir;

    public FlujoWorkflowServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "wfd-flujo-" + Guid.NewGuid());
        Directory.CreateDirectory(_tempDir);
        FlujoWorkflowService.StorageDirectoryOverride = _tempDir;
        FlujoWorkflowService.Reset();
    }

    public void Dispose()
    {
        FlujoWorkflowService.Reset();
        FlujoWorkflowService.StorageDirectoryOverride = null;
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    [Fact]
    public void Load_sin_archivo_devuelve_datos_por_defecto()
    {
        var data = FlujoWorkflowService.Load();

        Assert.Equal("Nuevo Proceso", data.NombreFlujoActual);
        Assert.NotEmpty(data.PasosActuales);
        Assert.NotEmpty(data.Automatizaciones);
    }

    [Fact]
    public void Save_persiste_y_recarga_datos()
    {
        var custom = new FlujoWorkflowData
        {
            NombreFlujoActual = "Flujo QA",
            PasosActuales = [new FlowStepData { Tipo = "Accion", Descripcion = "Enviar aviso" }],
            Automatizaciones = []
        };

        FlujoWorkflowService.Save(custom);
        FlujoWorkflowService.InvalidateCache();
        var loaded = FlujoWorkflowService.Load();

        Assert.Equal("Flujo QA", loaded.NombreFlujoActual);
        Assert.Single(loaded.PasosActuales);
        Assert.Equal("Enviar aviso", loaded.PasosActuales[0].Descripcion);
    }

    [Fact]
    public void Reset_elimina_almacenamiento_local()
    {
        FlujoWorkflowService.Save(new FlujoWorkflowData { NombreFlujoActual = "Temporal" });
        FlujoWorkflowService.Reset();

        Assert.False(File.Exists(Path.Combine(_tempDir, "flujos-workflow.json")));
    }
}
