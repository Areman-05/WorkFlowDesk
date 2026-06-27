namespace WorkFlowDesk.Services.Interfaces;

/// <summary>Búsqueda global en entidades principales.</summary>
public interface IGlobalSearchService
{
    Task<IReadOnlyList<GlobalSearchResult>> BuscarAsync(string termino, int limite = 20);
}

public sealed class GlobalSearchResult
{
    public string Tipo { get; init; } = string.Empty;
    public int Id { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string Subtitulo { get; init; } = string.Empty;
    public string Seccion { get; init; } = string.Empty;
}
