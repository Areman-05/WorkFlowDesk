using WorkFlowDesk.Services.Exceptions;

namespace WorkFlowDesk.ViewModel.Base;

public static class ExceptionHandler
{
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
