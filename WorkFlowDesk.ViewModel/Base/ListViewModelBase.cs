namespace WorkFlowDesk.ViewModel.Base;

/// <summary>Base para ViewModels de listados con estado vacío y carga.</summary>
public abstract class ListViewModelBase : ViewModelBase
{
    public bool HayElementos => ObtenerCantidadElementos() > 0;

    public bool MostrarEstadoVacio => !IsLoading && !HayElementos;

    protected void NotificarEstadoLista()
    {
        OnPropertyChanged(nameof(HayElementos));
        OnPropertyChanged(nameof(MostrarEstadoVacio));
    }

    protected abstract int ObtenerCantidadElementos();
}
