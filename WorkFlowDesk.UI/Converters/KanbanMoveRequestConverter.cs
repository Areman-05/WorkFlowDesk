using System.Globalization;
using System.Windows.Data;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.ViewModel.Models;

namespace WorkFlowDesk.UI.Converters;

/// <summary>Combina tarjeta y estado destino en una solicitud Kanban.</summary>
public class KanbanMoveRequestConverter : IMultiValueConverter
{
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2 ||
            values[0] is not TareaListItem item ||
            values[1] is not EstadoTarea nuevoEstado)
            return null;

        return new KanbanMoveRequest { Item = item, NuevoEstado = nuevoEstado };
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
