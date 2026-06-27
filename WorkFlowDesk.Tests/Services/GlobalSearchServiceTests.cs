using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Services;
using WorkFlowDesk.Tests.Infrastructure;

namespace WorkFlowDesk.Tests.Services;

public class GlobalSearchServiceTests
{
    [Fact]
    public async Task BuscarAsync_encuentra_tarea_por_titulo()
    {
        await using var context = await TestDbContextFactory.CreateSeededAsync();
        var tareaService = new TareaService(context, TestServices.ActivityLog, TestServices.Automation);
        await tareaService.CreateAsync(new Tarea
        {
            Titulo = "Informe único XYZ",
            Estado = EstadoTarea.Pendiente,
            Prioridad = PrioridadTarea.Baja
        });

        var service = new GlobalSearchService(context);
        var results = await service.BuscarAsync("XYZ");

        Assert.Contains(results, r => r.Tipo == "Tarea" && r.Titulo.Contains("XYZ"));
    }
}
