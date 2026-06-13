using WorkFlowDesk.Common.Security;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Services;
using WorkFlowDesk.Tests.Infrastructure;

namespace WorkFlowDesk.Tests.Services;

public class UsuarioServiceTests
{
    [Fact]
    public async Task RegistrarAsync_crea_usuario_con_rol_empleado()
    {
        await using var context = await TestDbContextFactory.CreateSeededAsync();
        var service = new UsuarioService(context, new PasswordHasherService());

        var usuario = await service.RegistrarAsync(
            "nuevo.empleado",
            "nuevo@workflowdesk.local",
            "Nuevo Empleado",
            "Empleado123",
            TipoRol.Empleado);

        Assert.Equal("nuevo.empleado", usuario.NombreUsuario);
        Assert.NotNull(usuario.Rol);
        Assert.Equal(TipoRol.Empleado, usuario.Rol.TipoRol);
    }

    [Theory]
    [InlineData(TipoRol.Admin)]
    [InlineData(TipoRol.Supervisor)]
    [InlineData(TipoRol.Empleado)]
    public async Task RegistrarAsync_asigna_el_rol_seleccionado(TipoRol tipoRol)
    {
        await using var context = await TestDbContextFactory.CreateSeededAsync();
        var service = new UsuarioService(context, new PasswordHasherService());
        var suffix = Guid.NewGuid().ToString("N")[..8];

        var usuario = await service.RegistrarAsync(
            $"user.{suffix}",
            $"{suffix}@workflowdesk.local",
            $"Usuario {suffix}",
            "Password1",
            tipoRol);

        Assert.Equal(tipoRol, usuario.Rol.TipoRol);
    }

    [Fact]
    public async Task RegistrarAsync_usuario_duplicado_lanza_excepcion()
    {
        await using var context = await TestDbContextFactory.CreateSeededAsync();
        var service = new UsuarioService(context, new PasswordHasherService());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.RegistrarAsync("admin", "otro@workflowdesk.local", "Otro Admin", "Password1", TipoRol.Admin));
    }

    [Fact]
    public async Task RegistrarAsync_email_duplicado_lanza_excepcion()
    {
        await using var context = await TestDbContextFactory.CreateSeededAsync();
        var service = new UsuarioService(context, new PasswordHasherService());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.RegistrarAsync("otro.admin", "admin@workflowdesk.local", "Otro Admin", "Password1", TipoRol.Admin));
    }
}
