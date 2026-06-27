using WorkFlowDesk.Common.Authorization;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.Tests.Authorization;

public class RolePermissionsTests
{
    public RolePermissionsTests()
    {
        SessionService.ClearSession();
    }

    [Fact]
    public void Sin_sesion_no_puede_acceder_a_secciones_protegidas()
    {
        Assert.False(RolePermissions.CanAccessEmpleados);
        Assert.False(RolePermissions.CanAccessConfiguracion);
        Assert.False(RolePermissions.CanExportData);
    }

    [Fact]
    public void Admin_tiene_acceso_completo()
    {
        SessionService.SetCurrentUser(CrearUsuario(TipoRol.Admin));

        Assert.True(RolePermissions.CanAccessDashboard);
        Assert.True(RolePermissions.CanAccessEmpleados);
        Assert.True(RolePermissions.CanAccessProyectos);
        Assert.True(RolePermissions.CanAccessTareas);
        Assert.True(RolePermissions.CanAccessClientes);
        Assert.True(RolePermissions.CanAccessReportes);
        Assert.True(RolePermissions.CanAccessOptimizacion);
        Assert.True(RolePermissions.CanAccessConfiguracion);
        Assert.True(RolePermissions.CanManageConfiguration);
        Assert.True(RolePermissions.CanExportData);
        Assert.False(RolePermissions.IsReadOnlyUser);
    }

    [Fact]
    public void Supervisor_puede_gestionar_pero_no_configuracion()
    {
        SessionService.SetCurrentUser(CrearUsuario(TipoRol.Supervisor));

        Assert.True(RolePermissions.CanAccessEmpleados);
        Assert.True(RolePermissions.CanManageProyectos);
        Assert.True(RolePermissions.CanExportData);
        Assert.False(RolePermissions.CanAccessConfiguracion);
        Assert.False(RolePermissions.CanManageConfiguration);
        Assert.False(RolePermissions.IsReadOnlyUser);
    }

    [Fact]
    public void Empleado_solo_accede_a_dashboard_y_tareas()
    {
        SessionService.SetCurrentUser(CrearUsuario(TipoRol.Empleado));

        Assert.True(RolePermissions.CanAccessDashboard);
        Assert.True(RolePermissions.CanAccessTareas);
        Assert.False(RolePermissions.CanAccessEmpleados);
        Assert.False(RolePermissions.CanAccessProyectos);
        Assert.False(RolePermissions.CanAccessClientes);
        Assert.False(RolePermissions.CanAccessReportes);
        Assert.False(RolePermissions.CanAccessOptimizacion);
        Assert.False(RolePermissions.CanManageTareas);
        Assert.True(RolePermissions.IsReadOnlyUser);
    }

    private static Usuario CrearUsuario(TipoRol tipoRol) => new()
    {
        Id = 1,
        NombreUsuario = tipoRol.ToString().ToLowerInvariant(),
        NombreCompleto = $"Usuario {tipoRol}",
        Rol = new Rol { Id = (int)tipoRol, Nombre = tipoRol.ToString(), TipoRol = tipoRol }
    };
}
