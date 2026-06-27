using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Exceptions;
using WorkFlowDesk.Services.Services;
using WorkFlowDesk.Tests.Infrastructure;

namespace WorkFlowDesk.Tests.Services;

public class EmpleadoServiceTests
{
    [Fact]
    public async Task CreateAsync_persiste_empleado()
    {
        await using var context = await TestDbContextFactory.CreateSeededAsync();
        var service = new EmpleadoService(context);
        var empleado = new Empleado
        {
            Nombre = "Ana",
            Apellidos = "López",
            Email = "ana.lopez@test.local",
            Estado = EstadoEmpleado.Activo
        };

        var creado = await service.CreateAsync(empleado);

        Assert.True(creado.Id > 0);
        var todos = await service.GetAllAsync();
        Assert.Contains(todos, e => e.Email == "ana.lopez@test.local");
    }

    [Fact]
    public async Task CreateAsync_rechaza_email_duplicado()
    {
        await using var context = await TestDbContextFactory.CreateSeededAsync();
        var service = new EmpleadoService(context);
        var baseEmpleado = new Empleado
        {
            Nombre = "Uno",
            Apellidos = "Test",
            Email = "dup@test.local",
            Estado = EstadoEmpleado.Activo
        };
        await service.CreateAsync(baseEmpleado);

        await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(new Empleado
        {
            Nombre = "Dos",
            Apellidos = "Test",
            Email = "dup@test.local",
            Estado = EstadoEmpleado.Activo
        }));
    }

    [Fact]
    public async Task GetActivosAsync_excluye_inactivos()
    {
        await using var context = await TestDbContextFactory.CreateSeededAsync();
        var service = new EmpleadoService(context);
        await service.CreateAsync(new Empleado
        {
            Nombre = "Inactivo",
            Apellidos = "Demo",
            Email = "inactivo@test.local",
            Estado = EstadoEmpleado.Inactivo
        });

        var activos = await service.GetActivosAsync();

        Assert.All(activos, e => Assert.Equal(EstadoEmpleado.Activo, e.Estado));
        Assert.DoesNotContain(activos, e => e.Email == "inactivo@test.local");
    }
}
