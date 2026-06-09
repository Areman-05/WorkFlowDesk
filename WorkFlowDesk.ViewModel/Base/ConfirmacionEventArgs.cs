namespace WorkFlowDesk.ViewModel.Base;

/// <summary>Argumentos para solicitar confirmación al usuario desde la vista.</summary>
public class ConfirmacionEventArgs : EventArgs
{
    public required string Mensaje { get; init; }
    public string Titulo { get; init; } = "Confirmar";
    public bool Confirmado { get; set; }
}
