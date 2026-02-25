using System.ComponentModel;
using System.Reflection;

namespace WorkFlowDesk.Common.Helpers;

/// <summary>Utilidades para enums (descripción, parseo, valores).</summary>
public static class EnumHelper
{
    /// <summary>Obtiene la descripción del valor enum o su ToString.</summary>
    public static string GetDescription(Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        if (field == null) return value.ToString();

        var attribute = field.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }

    /// <summary>Devuelve todos los valores del enum.</summary>
    public static IEnumerable<T> GetAllValues<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }

    /// <summary>Intenta parsear una cadena al enum (insensible a mayúsculas).</summary>
    public static T? ParseEnum<T>(string value) where T : struct, Enum
    {
        if (Enum.TryParse<T>(value, true, out var result))
        {
            return result;
        }
        return null;
    }

    /// <summary>Convierte el nombre del enum a texto legible (reemplaza _ por espacio).</summary>
    public static string ToDisplayString(Enum value)
    {
        return value.ToString().Replace("_", " ");
    }
}
