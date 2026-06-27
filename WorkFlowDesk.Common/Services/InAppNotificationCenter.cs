using System.Collections.ObjectModel;

namespace WorkFlowDesk.Common.Services;

public enum AppNotificationKind
{
    Info,
    Success,
    Warning,
    Error
}

public sealed class AppNotificationItem
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string Titulo { get; init; } = "WorkFlowDesk";
    public string Mensaje { get; init; } = string.Empty;
    public AppNotificationKind Tipo { get; init; } = AppNotificationKind.Info;
    public DateTime Fecha { get; init; } = DateTime.Now;
    public bool Leida { get; set; }
    public string? Seccion { get; init; }

    public string IconoGlyph => Tipo switch
    {
        AppNotificationKind.Success => "\uE73E",
        AppNotificationKind.Warning => "\uE7BA",
        AppNotificationKind.Error => "\uE783",
        _ => "\uE789"
    };

    public string TiempoRelativo
    {
        get
        {
            var diff = DateTime.Now - Fecha;
            if (diff.TotalMinutes < 1) return "Ahora";
            if (diff.TotalMinutes < 60) return $"Hace {(int)diff.TotalMinutes} min";
            if (diff.TotalHours < 24) return $"Hace {(int)diff.TotalHours} h";
            return Fecha.ToString("dd MMM HH:mm");
        }
    }
}

/// <summary>Centro de notificaciones in-app (campana superior).</summary>
public static class InAppNotificationCenter
{
    public static ObservableCollection<AppNotificationItem> Items { get; } = new();

    public static int NoLeidas => Items.Count(i => !i.Leida);

    public static event EventHandler? Changed;

    public static void Add(string mensaje, AppNotificationKind tipo = AppNotificationKind.Info,
        string titulo = "WorkFlowDesk", string? seccion = null)
    {
        Items.Insert(0, new AppNotificationItem
        {
            Titulo = titulo,
            Mensaje = mensaje,
            Tipo = tipo,
            Seccion = seccion
        });

        while (Items.Count > 30)
            Items.RemoveAt(Items.Count - 1);

        NotifyChanged();
    }

    public static void AddContextual(string mensaje, AppNotificationKind tipo, string titulo, string? seccion = null)
    {
        if (Items.Any(i => !i.Leida && i.Mensaje == mensaje && i.Titulo == titulo))
            return;

        Add(mensaje, tipo, titulo, seccion);
    }

    public static void MarkAllRead()
    {
        foreach (var item in Items)
            item.Leida = true;

        NotifyChanged();
    }

    public static void MarkRead(AppNotificationItem item)
    {
        item.Leida = true;
        NotifyChanged();
    }

    public static void Remove(AppNotificationItem item)
    {
        if (Items.Remove(item))
            NotifyChanged();
    }

    public static void RemoveAll()
    {
        if (Items.Count == 0)
            return;

        Items.Clear();
        NotifyChanged();
    }

    private static void NotifyChanged() => Changed?.Invoke(null, EventArgs.Empty);
}
