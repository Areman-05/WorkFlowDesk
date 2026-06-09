using CommunityToolkit.Mvvm.ComponentModel;

namespace WorkFlowDesk.ViewModel.Base;

/// <summary>Base para todos los ViewModels con soporte de carga y mensaje de error.</summary>
public abstract class ViewModelBase : ObservableObject
{
    private bool _isLoading;
    private string? _errorMessage;

    public event EventHandler<ConfirmacionEventArgs>? ConfirmacionSolicitada;

    /// <summary>Solicita confirmación al usuario a través de la vista.</summary>
    protected bool SolicitarConfirmacion(string mensaje, string titulo = "Confirmar")
    {
        var args = new ConfirmacionEventArgs { Mensaje = mensaje, Titulo = titulo };
        ConfirmacionSolicitada?.Invoke(this, args);
        return args.Confirmado;
    }

    /// <summary>Indica si la vista está realizando una operación asíncrona.</summary>
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    /// <summary>Mensaje de error a mostrar en la vista, o null si no hay error.</summary>
    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }
}
