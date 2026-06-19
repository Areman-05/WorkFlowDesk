using WorkFlowDesk.Common.Services;
using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.Common.Authorization;

/// <summary>Reglas de acceso por rol para navegación y acciones de la aplicación.</summary>
public static class RolePermissions
{
    public static bool CanAccessDashboard => SessionService.IsAuthenticated;

    public static bool CanAccessEmpleados =>
        SessionService.IsAdmin() || SessionService.IsSupervisor();

    public static bool CanAccessProyectos =>
        SessionService.IsAdmin() || SessionService.IsSupervisor();

    public static bool CanAccessTareas => SessionService.IsAuthenticated;

    public static bool CanAccessClientes =>
        SessionService.IsAdmin() || SessionService.IsSupervisor();

    public static bool CanAccessReportes =>
        SessionService.IsAdmin() || SessionService.IsSupervisor();

    public static bool CanAccessOptimizacion =>
        SessionService.IsAdmin() || SessionService.IsSupervisor();

    public static bool CanAccessConfiguracion => SessionService.IsAdmin();

    public static bool CanManageEmpleados =>
        SessionService.IsAdmin() || SessionService.IsSupervisor();

    public static bool CanManageProyectos =>
        SessionService.IsAdmin() || SessionService.IsSupervisor();

    public static bool CanManageTareas =>
        SessionService.IsAdmin() || SessionService.IsSupervisor();

    public static bool CanManageClientes =>
        SessionService.IsAdmin() || SessionService.IsSupervisor();

    public static bool CanExportData =>
        SessionService.IsAdmin() || SessionService.IsSupervisor();

    public static bool CanManageConfiguration => SessionService.IsAdmin();

    public static bool IsReadOnlyUser =>
        SessionService.HasRole(TipoRol.Empleado) && !SessionService.IsSupervisor();
}
