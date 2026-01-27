namespace WorkFlowDesk.Common.Extensions;

public static class NumberExtensions
{
    /// <summary>Devuelve el valor limitado al rango [min, max].</summary>
    public static int Clamp(this int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    /// <summary>Indica si el valor está en el rango [min, max] (incluidos).</summary>
    public static bool IsBetween(this int value, int min, int max)
    {
        return value >= min && value <= max;
    }
}
