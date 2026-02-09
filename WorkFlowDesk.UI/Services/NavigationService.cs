using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace WorkFlowDesk.UI.Services;

/// <summary>Gestiona la navegación entre vistas y el historial para volver atrás.</summary>
public class NavigationService
{
    private ContentControl? _contentArea;
    private readonly Stack<UserControl> _navigationHistory = new();
    private UserControl? _currentView;

    /// <summary>Asigna el área de contenido donde se mostrarán las vistas.</summary>
    public void Initialize(ContentControl contentArea)
    {
        _contentArea = contentArea;
    }

    /// <summary>Navega a la vista indicada y la muestra en el área de contenido.</summary>
    public void NavigateTo(UserControl view)
    {
        if (_currentView != null && _currentView != view)
        {
            _navigationHistory.Push(_currentView);
        }

        _currentView = view;
        _contentArea?.SetCurrentValue(ContentControl.ContentProperty, view);
    }

    public void NavigateTo<T>() where T : UserControl, new()
    {
        NavigateTo(new T());
    }

    public bool CanGoBack()
    {
        return _navigationHistory.Count > 0;
    }

    public void GoBack()
    {
        if (CanGoBack())
        {
            var previousView = _navigationHistory.Pop();
            _currentView = previousView;
            _contentArea?.SetCurrentValue(ContentControl.ContentProperty, previousView);
        }
    }

    public void ClearHistory()
    {
        _navigationHistory.Clear();
    }
}
