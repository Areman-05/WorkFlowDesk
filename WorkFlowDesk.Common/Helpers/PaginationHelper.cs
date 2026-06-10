namespace WorkFlowDesk.Common.Helpers;

/// <summary>Aplica paginación en memoria sobre una colección.</summary>
public class PaginationHelper
{
    private int _paginaActual = 1;

    public int PaginaActual
    {
        get => _paginaActual;
        set => _paginaActual = value < 1 ? 1 : value;
    }

    public int TamañoPagina { get; set; } = 20;

    public int TotalItems { get; private set; }

    public int TotalPaginas => TamañoPagina > 0
        ? Math.Max(1, (int)Math.Ceiling((double)TotalItems / TamañoPagina))
        : 1;

    public bool TienePaginaAnterior => PaginaActual > 1;

    public bool TienePaginaSiguiente => PaginaActual < TotalPaginas && TotalItems > 0;

    public string Resumen => TotalItems == 0
        ? "Sin registros"
        : $"Página {PaginaActual} de {TotalPaginas} ({TotalItems} registros)";

    public void Reiniciar() => PaginaActual = 1;

    public void PaginaAnterior()
    {
        if (TienePaginaAnterior)
            PaginaActual--;
    }

    public void PaginaSiguiente()
    {
        if (TienePaginaSiguiente)
            PaginaActual++;
    }

    public IEnumerable<T> Aplicar<T>(IEnumerable<T> source)
    {
        var lista = source.ToList();
        TotalItems = lista.Count;

        if (TotalItems == 0)
        {
            PaginaActual = 1;
            return Enumerable.Empty<T>();
        }

        if (PaginaActual > TotalPaginas)
            PaginaActual = TotalPaginas;

        return lista
            .Skip((PaginaActual - 1) * TamañoPagina)
            .Take(TamañoPagina);
    }
}
