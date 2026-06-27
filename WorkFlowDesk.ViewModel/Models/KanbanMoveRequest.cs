using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.ViewModel.Models;

/// <summary>Solicitud para mover una tarjeta Kanban a otro estado.</summary>
public class KanbanMoveRequest
{
    public required TareaListItem Item { get; init; }
    public EstadoTarea NuevoEstado { get; init; }
}
