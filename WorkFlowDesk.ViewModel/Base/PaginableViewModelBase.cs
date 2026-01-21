using System.Collections.ObjectModel;

namespace WorkFlowDesk.ViewModel.Base;

public abstract class PaginableViewModelBase<T> : ViewModelBase
{
    private int _paginaActual = 1;
    private int _tamañoPagina = 20;
    private int _totalItems;
    private ObservableCollection<T> _itemsPagina = new();

    public int PaginaActual
    {
        get => _paginaActual;
        set
        {
            SetProperty(ref _paginaActual, value);
            CargarPagina();
        }
    }

    public int TamañoPagina
    {
        get => _tamañoPagina;
        set
        {
            SetProperty(ref _tamañoPagina, value);
            PaginaActual = 1;
            CargarPagina();
        }
    }

    public int TotalItems
    {
        get => _totalItems;
        set => SetProperty(ref _totalItems, value);
    }

    public int TotalPaginas => (int)Math.Ceiling((double)TotalItems / TamañoPagina);

    public ObservableCollection<T> ItemsPagina
    {
        get => _itemsPagina;
        set => SetProperty(ref _itemsPagina, value);
    }

    public bool TienePaginaAnterior => PaginaActual > 1;
    public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;

    protected abstract void CargarPagina();

    public void IrAPaginaAnterior()
    {
        if (TienePaginaAnterior)
        {
            PaginaActual--;
        }
    }

    public void IrAPaginaSiguiente()
    {
        if (TienePaginaSiguiente)
        {
            PaginaActual++;
        }
    }

    public void IrAPrimeraPagina()
    {
        PaginaActual = 1;
    }

    public void IrAUltimaPagina()
    {
        PaginaActual = TotalPaginas;
    }
}
