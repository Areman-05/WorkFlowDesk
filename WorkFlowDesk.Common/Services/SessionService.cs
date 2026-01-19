using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.Common.Services;

public static class SessionService
{
    private static Usuario? _currentUser;

    public static Usuario? CurrentUser
    {
        get => _currentUser;
        private set => _currentUser = value;
    }

    public static bool IsAuthenticated => CurrentUser != null;

    public static void SetCurrentUser(Usuario usuario)
    {
        CurrentUser = usuario;
    }

    public static void ClearSession()
    {
        CurrentUser = null;
    }

    public static bool HasRole(TipoRol rol)
    {
        return CurrentUser?.Rol?.TipoRol == rol;
    }

    public static bool IsAdmin()
    {
        return HasRole(TipoRol.Admin);
    }

    public static bool IsSupervisor()
    {
        return HasRole(TipoRol.Supervisor) || IsAdmin();
    }
}
