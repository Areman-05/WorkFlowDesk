namespace WorkFlowDesk.Common.Helpers;

/// <summary>Utilidades para enums en UI y exportaciones.</summary>
public static class EnumHelper
{
    /// <summary>Convierte el nombre del enum a texto legible (reemplaza _ por espacio).</summary>
    public static string ToDisplayString(Enum value) =>
        value.ToString().Replace("_", " ");
}
