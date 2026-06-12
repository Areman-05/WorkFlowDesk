using WorkFlowDesk.Common.Security;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Services;
using WorkFlowDesk.Tests.Infrastructure;

namespace WorkFlowDesk.Tests.Services;

public class AuthenticationServiceTests
{
    [Fact]
    public async Task AutenticarAsync_credenciales_validas_devuelve_usuario()
    {
        await using var context = await TestDbContextFactory.CreateSeededAsync();
        var service = new AuthenticationService(context, new PasswordHasherService());

        var usuario = await service.AutenticarAsync("admin", "Admin123");

        Assert.NotNull(usuario);
        Assert.Equal("admin", usuario.NombreUsuario);
        Assert.NotNull(usuario.Rol);
        Assert.Equal(TipoRol.Admin, usuario.Rol.TipoRol);
    }

    [Fact]
    public async Task AutenticarAsync_password_incorrecta_devuelve_null()
    {
        await using var context = await TestDbContextFactory.CreateSeededAsync();
        var service = new AuthenticationService(context, new PasswordHasherService());

        var usuario = await service.AutenticarAsync("admin", "mal");

        Assert.Null(usuario);
    }

    [Fact]
    public async Task CambiarPasswordAsync_actualiza_hash_si_password_es_correcta()
    {
        await using var context = await TestDbContextFactory.CreateSeededAsync();
        var service = new AuthenticationService(context, new PasswordHasherService());
        var admin = await service.AutenticarAsync("admin", "Admin123");
        Assert.NotNull(admin);

        var cambiado = await service.CambiarPasswordAsync(admin.Id, "Admin123", "NuevaPass1");

        Assert.True(cambiado);
        var reauth = await service.AutenticarAsync("admin", "NuevaPass1");
        Assert.NotNull(reauth);
    }
}
