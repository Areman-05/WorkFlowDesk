using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.Services.Services;
using WorkFlowDesk.Tests.Infrastructure;

namespace WorkFlowDesk.Tests.Services;

public class ReporteServiceTests
{
    [Fact]
    public async Task ObtenerEstadisticasTareasAsync_incluye_totales_por_estado()
    {
        await using var context = await TestDbContextFactory.CreateSeededAsync();
        var tareaService = new TareaService(context, new NoOpActivityLog(), new NoOpAutomation());
        await tareaService.CreateAsync(new Tarea
        {
            Titulo = "Extra pendiente",
            Estado = EstadoTarea.Pendiente,
            Prioridad = PrioridadTarea.Baja
        });

        var service = new ReporteService(context);
        var stats = await service.ObtenerEstadisticasTareasAsync();

        Assert.True(stats["Total"] >= 1);
        Assert.True(stats.ContainsKey("Pendientes"));
        Assert.True(stats.ContainsKey("Completadas"));
        Assert.Equal(stats["Total"],
            stats["Pendientes"] + stats["En Progreso"] + stats["Completadas"] + stats["Canceladas"]);
    }

    [Fact]
    public async Task ObtenerEstadisticasEmpleadosAsync_devuelve_claves_esperadas()
    {
        await using var context = await TestDbContextFactory.CreateSeededAsync();
        var service = new ReporteService(context);

        var stats = await service.ObtenerEstadisticasEmpleadosAsync();

        Assert.Equal(["Activos", "Baja", "Inactivos", "Total", "Vacaciones"], stats.Keys.OrderBy(k => k));
        Assert.True(stats["Total"] >= stats["Activos"]);
    }

    [Fact]
    public async Task ObtenerEstadisticasProyectosAsync_devuelve_claves_esperadas()
    {
        await using var context = await TestDbContextFactory.CreateSeededAsync();
        var service = new ReporteService(context);

        var stats = await service.ObtenerEstadisticasProyectosAsync();

        Assert.Contains("Total", stats.Keys);
        Assert.Contains("En Progreso", stats.Keys);
        Assert.Contains("Completados", stats.Keys);
    }
}
