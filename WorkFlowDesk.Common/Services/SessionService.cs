using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.Common.Services;

/// <summary>Gestiona el usuario y estado de la sesión actual de la aplicación.</summary>
public static class SessionService
{
    private static Usuario? _currentUser;
    private static DateTime? _sessionStartTime;

    /// <summary>Usuario actualmente autenticado, o null si no hay sesión.</summary>
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

    /// <summary>Establece el usuario actual y marca el inicio de sesión.</summary>
    public static void SetCurrentUser(Usuario usuario)
    {
        CurrentUser = usuario;
        _sessionStartTime = DateTime.Now;
        SessionStarted?.Invoke(null, EventArgs.Empty);
    }

    /// <summary>Cierra la sesión y borra el usuario actual.</summary>
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
