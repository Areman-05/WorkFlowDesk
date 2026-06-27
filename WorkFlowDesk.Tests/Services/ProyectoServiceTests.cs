using Microsoft.EntityFrameworkCore;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Services;
using WorkFlowDesk.Tests.Infrastructure;

namespace WorkFlowDesk.Tests.Services;

public class ProyectoServiceTests
{
    [Fact]
    public async Task CreateAsync_persiste_proyecto()
    {
        await using var context = await TestDbContextFactory.CreateSeededAsync();
        var service = new ProyectoService(context);
        var cliente = await context.Clientes.FirstAsync();
        var proyecto = new Proyecto
        {
            Nombre = "Portal Web",
            Descripcion = "Rediseño",
            Estado = EstadoProyecto.Planificacion,
            ClienteId = cliente.Id
        };

        var creado = await service.CreateAsync(proyecto);

        Assert.True(creado.Id > 0);
        Assert.NotEqual(default, creado.FechaCreacion);
    }

    [Fact]
    public async Task GetByEstadoAsync_filtra_por_estado()
    {
        await using var context = await TestDbContextFactory.CreateSeededAsync();
        var service = new ProyectoService(context);
        var cliente = await context.Clientes.FirstAsync();
        await service.CreateAsync(new Proyecto
        {
            Nombre = "En curso",
            Estado = EstadoProyecto.EnProgreso,
            ClienteId = cliente.Id
        });

        var enProgreso = await service.GetByEstadoAsync(EstadoProyecto.EnProgreso);

        Assert.All(enProgreso, p => Assert.Equal(EstadoProyecto.EnProgreso, p.Estado));
        Assert.Contains(enProgreso, p => p.Nombre == "En curso");
    }
}
