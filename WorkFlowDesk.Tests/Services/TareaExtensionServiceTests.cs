using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Services;
using WorkFlowDesk.Tests.Infrastructure;

namespace WorkFlowDesk.Tests.Services;

public class TareaExtensionServiceTests
{
    [Fact]
    public async Task AgregarSubtarea_y_registrar_tiempo()
    {
        await using var context = await TestDbContextFactory.CreateSeededAsync();
        var tareaService = new TareaService(context, TestServices.ActivityLog, TestServices.Automation);
        var extension = new TareaExtensionService(context, TestServices.ActivityLog, TestServices.Automation);
        var tarea = await tareaService.CreateAsync(new Tarea
        {
            Titulo = "Con subtareas",
            Estado = EstadoTarea.Pendiente,
            Prioridad = PrioridadTarea.Media
        });

        var st = await extension.AgregarSubtareaAsync(tarea.Id, "Paso 1");
        await extension.RegistrarTiempoAsync(tarea.Id, 45, "Revisión", null);

        Assert.True(st.Id > 0);
        Assert.Equal(45, await extension.GetMinutosTotalesAsync(tarea.Id));
    }

    [Fact]
    public async Task Dependencia_bloquea_inicio_hasta_completar()
    {
        await using var context = await TestDbContextFactory.CreateSeededAsync();
        var tareaService = new TareaService(context, TestServices.ActivityLog, TestServices.Automation);
        var extension = new TareaExtensionService(context, TestServices.ActivityLog, TestServices.Automation);
        var bloqueante = await tareaService.CreateAsync(new Tarea
        {
            Titulo = "Bloqueante",
            Estado = EstadoTarea.Pendiente,
            Prioridad = PrioridadTarea.Alta
        });
        var dependiente = await tareaService.CreateAsync(new Tarea
        {
            Titulo = "Dependiente",
            Estado = EstadoTarea.Pendiente,
            Prioridad = PrioridadTarea.Media
        });

        await extension.AgregarDependenciaAsync(dependiente.Id, bloqueante.Id);
        Assert.False(await extension.PuedeIniciarAsync(dependiente.Id));

        bloqueante.Estado = EstadoTarea.Completada;
        await tareaService.UpdateAsync(bloqueante);
        Assert.True(await extension.PuedeIniciarAsync(dependiente.Id));
    }
}
