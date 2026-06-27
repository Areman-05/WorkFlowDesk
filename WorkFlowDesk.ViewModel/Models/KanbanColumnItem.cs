using System.Collections.ObjectModel;
using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.ViewModel.Models;

/// <summary>Columna del tablero Kanban agrupada por estado.</summary>
public class KanbanColumnItem
{
    public string Titulo { get; init; } = string.Empty;
    public EstadoTarea Estado { get; init; }
    public EstadoTarea? EstadoSiguiente { get; init; }
    public string? TextoMoverSiguiente { get; init; }
    public ObservableCollection<TareaListItem> Tarjetas { get; } = new();
}
