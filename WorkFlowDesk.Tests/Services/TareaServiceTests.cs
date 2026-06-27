using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Services;
using WorkFlowDesk.Tests.Infrastructure;

namespace WorkFlowDesk.Tests.Services;

public class TareaServiceTests
{
    [Fact]
    public async Task CreateAsync_persiste_tarea_con_fecha_creacion()
    {
        await using var context = await TestDbContextFactory.CreateSeededAsync();
        var service = CreateService(context);
        var tarea = new Tarea
        {
            Titulo = "Revisar informe",
            Estado = EstadoTarea.Pendiente,
            Prioridad = PrioridadTarea.Media
        };

        var creada = await service.CreateAsync(tarea);

        Assert.True(creada.Id > 0);
        Assert.NotEqual(default, creada.FechaCreacion);
        var todas = await service.GetAllAsync();
        Assert.Contains(todas, t => t.Titulo == "Revisar informe");
    }

    [Fact]
    public async Task GetByEstadoAsync_filtra_correctamente()
    {
        await using var context = await TestDbContextFactory.CreateSeededAsync();
        var service = CreateService(context);
        await service.CreateAsync(new Tarea { Titulo = "Pendiente A", Estado = EstadoTarea.Pendiente, Prioridad = PrioridadTarea.Baja });
        await service.CreateAsync(new Tarea { Titulo = "Completada B", Estado = EstadoTarea.Completada, Prioridad = PrioridadTarea.Baja });

        var pendientes = await service.GetByEstadoAsync(EstadoTarea.Pendiente);

        Assert.All(pendientes, t => Assert.Equal(EstadoTarea.Pendiente, t.Estado));
        Assert.Contains(pendientes, t => t.Titulo == "Pendiente A");
    }

    [Fact]
    public async Task DeleteAsync_marca_tarea_como_cancelada()
    {
        await using var context = await TestDbContextFactory.CreateSeededAsync();
        var service = CreateService(context);
        var creada = await service.CreateAsync(new Tarea
        {
            Titulo = "Eliminar",
            Estado = EstadoTarea.Pendiente,
            Prioridad = PrioridadTarea.Baja
        });

        await service.DeleteAsync(creada.Id);

        var tarea = await service.GetByIdAsync(creada.Id);
        Assert.NotNull(tarea);
        Assert.Equal(EstadoTarea.Cancelada, tarea.Estado);
    }

    private static TareaService CreateService(WorkFlowDesk.Data.ApplicationDbContext context) =>
        new(context, new NoOpActivityLog(), new NoOpAutomation());
}
