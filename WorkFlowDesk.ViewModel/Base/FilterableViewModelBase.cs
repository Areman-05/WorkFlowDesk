using System.Collections.ObjectModel;
using System.Linq;

namespace WorkFlowDesk.ViewModel.Base;

public abstract class FilterableViewModelBase<T> : ViewModelBase
{
    private string _filtroTexto = string.Empty;
    private ObservableCollection<T> _itemsFiltrados = new();
    private ObservableCollection<T> _itemsOriginales = new();

    public string FiltroTexto
    {
        get => _filtroTexto;
        set
        {
            SetProperty(ref _filtroTexto, value);
            AplicarFiltros();
        }
    }

    public ObservableCollection<T> ItemsFiltrados
    {
        get => _itemsFiltrados;
        set => SetProperty(ref _itemsFiltrados, value);
    }

    protected ObservableCollection<T> ItemsOriginales
    {
        get => _itemsOriginales;
        set
        {
            _itemsOriginales = value;
            AplicarFiltros();
        }
    }

    protected virtual void AplicarFiltros()
    {
        var items = ItemsOriginales.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(FiltroTexto))
        {
            items = AplicarFiltroTexto(items, FiltroTexto);
        }

        ItemsFiltrados = new ObservableCollection<T>(items);
    }

    protected abstract IEnumerable<T> AplicarFiltroTexto(IEnumerable<T> items, string texto);
}
