using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.Common.Services;

public static class SessionService
{
    private static Usuario? _currentUser;
    private static DateTime? _sessionStartTime;

    public static Usuario? CurrentUser
    {
        get => _currentUser;
        private set => _currentUser = value;
    }

    public static bool IsAuthenticated => CurrentUser != null;

    public static DateTime? SessionStartTime => _sessionStartTime;

    public static TimeSpan? SessionDuration => _sessionStartTime.HasValue 
        ? DateTime.Now - _sessionStartTime.Value 
        : null;

    public static event EventHandler? SessionStarted;
    public static event EventHandler? SessionEnded;

    public static void SetCurrentUser(Usuario usuario)
    {
        CurrentUser = usuario;
        _sessionStartTime = DateTime.Now;
        SessionStarted?.Invoke(null, EventArgs.Empty);
    }

    public static void ClearSession()
    {
        var hadSession = IsAuthenticated;
        CurrentUser = null;
        _sessionStartTime = null;
        
        if (hadSession)
        {
            SessionEnded?.Invoke(null, EventArgs.Empty);
        }
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

    public static string GetUserName()
    {
        return CurrentUser?.NombreCompleto ?? "Usuario";
    }

    public static string GetUserRole()
    {
        return CurrentUser?.Rol?.Nombre ?? "Sin rol";
    }
}
