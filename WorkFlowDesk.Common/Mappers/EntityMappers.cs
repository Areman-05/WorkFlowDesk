using WorkFlowDesk.Common.Models;
using WorkFlowDesk.Domain.Entities;

namespace WorkFlowDesk.Common.Mappers;

public static class EntityMappers
{
    public static EmpleadoDto ToDto(this Empleado empleado)
    {
        return new EmpleadoDto
        {
            Id = empleado.Id,
            NombreCompleto = $"{empleado.Nombre} {empleado.Apellidos}",
            Email = empleado.Email,
            Departamento = empleado.Departamento,
            Cargo = empleado.Cargo,
            Estado = empleado.Estado.ToString()
        };
    }

    public static ProyectoDto ToDto(this Proyecto proyecto)
    {
        return new ProyectoDto
        {
            Id = proyecto.Id,
            Nombre = proyecto.Nombre,
            Descripcion = proyecto.Descripcion,
            Estado = proyecto.Estado.ToString(),
            ClienteNombre = proyecto.Cliente?.Nombre ?? string.Empty,
            ResponsableNombre = proyecto.Responsable != null 
                ? $"{proyecto.Responsable.Nombre} {proyecto.Responsable.Apellidos}" 
                : string.Empty,
            FechaInicio = proyecto.FechaInicio,
            FechaFin = proyecto.FechaFin
        };
    }

    public static TareaDto ToDto(this Tarea tarea)
    {
        return new TareaDto
        {
            Id = tarea.Id,
            Titulo = tarea.Titulo,
            Descripcion = tarea.Descripcion,
            Prioridad = tarea.Prioridad.ToString(),
            Estado = tarea.Estado.ToString(),
            AsignadoNombre = tarea.Asignado != null 
                ? $"{tarea.Asignado.Nombre} {tarea.Asignado.Apellidos}" 
                : string.Empty,
            ProyectoNombre = tarea.Proyecto?.Nombre ?? string.Empty,
            FechaVencimiento = tarea.FechaVencimiento
        };
    }
}
