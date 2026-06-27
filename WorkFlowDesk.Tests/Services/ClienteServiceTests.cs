using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Services;
using WorkFlowDesk.Tests.Infrastructure;

namespace WorkFlowDesk.Tests.Services;

public class ClienteServiceTests
{
    [Fact]
    public async Task CreateAsync_persiste_cliente_con_fecha()
    {
        await using var context = await TestDbContextFactory.CreateSeededAsync();
        var service = new ClienteService(context);
        var cliente = new Cliente
        {
            Nombre = "Acme Corp",
            Email = "contacto@acme.test",
            Activo = true
        };

        var creado = await service.CreateAsync(cliente);

        Assert.True(creado.Id > 0);
        Assert.NotEqual(default, creado.FechaCreacion);
    }

    [Fact]
    public async Task GetActivosAsync_filtra_inactivos()
    {
        await using var context = await TestDbContextFactory.CreateSeededAsync();
        var service = new ClienteService(context);
        await service.CreateAsync(new Cliente { Nombre = "Activo SA", Email = "a@test.local", Activo = true });
        await service.CreateAsync(new Cliente { Nombre = "Cerrado SL", Email = "b@test.local", Activo = false });

        var activos = await service.GetActivosAsync();

        Assert.All(activos, c => Assert.True(c.Activo));
        Assert.DoesNotContain(activos, c => c.Nombre == "Cerrado SL");
    }

    [Fact]
    public async Task DeleteAsync_desactiva_cliente()
    {
        await using var context = await TestDbContextFactory.CreateSeededAsync();
        var service = new ClienteService(context);
        var creado = await service.CreateAsync(new Cliente
        {
            Nombre = "Borrar",
            Email = "del@test.local",
            Activo = true
        });

        await service.DeleteAsync(creado.Id);

        var cliente = await service.GetByIdAsync(creado.Id);
        Assert.NotNull(cliente);
        Assert.False(cliente!.Activo);
    }
}
