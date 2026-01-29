using WorkFlowDesk.Services.Exceptions;

namespace WorkFlowDesk.ViewModel.Base;

/// <summary>Centraliza el manejo y el mensaje de excepciones para la capa de presentación.</summary>
public static class ExceptionHandler
{
    /// <summary>Devuelve un mensaje amigable según el tipo de excepción.</summary>
    public static string HandleException(Exception ex)
    {
        return ex switch
        {
            EntityNotFoundException entityEx => entityEx.Message,
            ValidationException validationEx => validationEx.Message,
            ServiceException serviceEx => serviceEx.Message,
            _ => $"Ha ocurrido un error inesperado: {ex.Message}"
        };
    }

    /// <summary>Registra la excepción (por defecto en Debug).</summary>
    public static void LogException(Exception ex)
    {
        // Aquí se puede agregar logging más adelante
        System.Diagnostics.Debug.WriteLine($"Exception: {ex.GetType().Name} - {ex.Message}");
        if (ex.InnerException != null)
        {
            System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
        }
    }
}
