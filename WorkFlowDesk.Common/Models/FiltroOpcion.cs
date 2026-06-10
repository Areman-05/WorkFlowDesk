namespace WorkFlowDesk.Common.Models;

/// <summary>Opción de filtro con etiqueta visible y valor subyacente.</summary>
public class FiltroOpcion<T>
{
    public string Etiqueta { get; init; } = string.Empty;
    public T? Valor { get; init; }
}
